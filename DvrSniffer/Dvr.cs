using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace DvrSniffer
{
    public class Dvr
    {
        private Socket _socket;
        private byte[] _headerBuffer = new byte[8];

        public DvrDeviceInformationPacket DeviceInformation { get; private set; }

        public DvrCameraInfoPacket CameraInfo { get; private set; }

        public Dvr()
        {
        }

        public void Connect(IPAddress ip, int port, string username, string password)
        {
            if (_socket != null)
            {
                throw new InvalidOperationException("Already connected");
            }

            var ipe = new IPEndPoint(ip, port);
            _socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ipe);

            // read header
            var header = new byte[0x40];
            var read = _socket.Receive(header);
            Debug.Assert(read == header.Length);
            Debug.Assert(header[0] == (byte)'h');
            Debug.Assert(header[1] == (byte)'e');
            Debug.Assert(header[2] == (byte)'a');
            Debug.Assert(header[3] == (byte)'d');

            // send login
            var loginPacket = new DvrLoginPacket();
            loginPacket.UserName = username;
            loginPacket.Password = password;
            loginPacket.MachineName = Environment.MachineName.ToLower();
            loginPacket.Send(_socket);

            var receive = true;
            while (receive)
            {
                var packet = ReadPacket();
                switch (packet?.PacketType)
                {
                    case DvrPacketType.CameraInfo:
                        CameraInfo = (DvrCameraInfoPacket)packet;
                        receive = false;
                        break;
                    case DvrPacketType.DeviceInformation:
                        DeviceInformation = (DvrDeviceInformationPacket)packet;
                        break;
                    default:
                        Console.WriteLine($"Unsupported packet type: {packet?.PacketType}");
                        break;
                }
            }
        }

        public byte[] RequestVideo(short cameraNumber)
        {
            var data = new byte[1 * 1024 * 1024]; // only receive 1 meg for now
            var offset = 0;
            var packetCount = 0;
            var request = new DvrRequestVideoDataPacket(cameraNumber);
            request.Send(_socket);
            var receive = true;
            while (receive)
            {
                var packet = ReadPacket();
                switch (packet?.PacketType)
                {
                    case DvrPacketType.VideoData:
                        var videoData = (DvrVideoDataPacket)packet;
                        if (offset + videoData.Data.Length > data.Length)
                        {
                            // this would overflow
                            receive = false;
                        }
                        else
                        {
                            Array.Copy(videoData.Data, 0, data, offset, videoData.Data.Length);
                            offset += videoData.Data.Length;
                        }

                        packetCount++;
                        if (packetCount > 50)
                        {
                            receive = false;
                        }
                        break;
                    default:
                        //receive = offset > 512*1024; // only quit if we have 500k
                        break;
                }
            }

            return data;
        }

        private DvrPacket ReadPacket()
        {
        top:
            var read = _socket.Receive(_headerBuffer);
            if (read != 8)
            {
                return null;
            }

            if (_headerBuffer[0] != 0x31 ||
                _headerBuffer[1] != 0x31 ||
                _headerBuffer[2] != 0x31 ||
                _headerBuffer[3] != 0x31)
            {
                return null;
            }

            var packetLength = BitConverter.ToInt32(_headerBuffer, 4);
            if (packetLength == 0)
            {
                return null;
            }

            var data = new byte[packetLength + 8];
            Array.Copy(_headerBuffer, 0, data, 0, _headerBuffer.Length);
            read = _socket.Receive(data, 8, packetLength, SocketFlags.None);

            var packetType = (DvrPacketType)BitConverter.ToInt32(data, 8);
            switch (packetType)
            {
                case DvrPacketType.CameraInfo:
                    return DvrCameraInfoPacket.FromData(data);
                case DvrPacketType.DeviceInformation:
                    return DvrDeviceInformationPacket.FromData(data);
                case DvrPacketType.VideoData:
                    var packet = DvrVideoDataPacket.FromData(data, 8, packetLength);
                    if (packet == null) goto top;
                    return packet;
                default:
                    return new DvrUnsupportedPacket(packetType);
            }
        }

        public void Close()
        {
            _socket?.Close();
        }
    }
}
