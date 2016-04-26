using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace DvrSniffer
{
    public class Dvr
    {
        private Socket _socket;
        private int _currentPos;
        private int _lastRead;
        private byte[] _buffer = new byte[4096];

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

        private DvrPacket ReadPacket()
        {
            if (_currentPos >= _lastRead - 8)
            {
                ReadMoreData();
            }

            var packetStart = _currentPos;
            Debug.Assert(_buffer[_currentPos++] == 0x31); // '1'
            Debug.Assert(_buffer[_currentPos++] == 0x31); // '1'
            Debug.Assert(_buffer[_currentPos++] == 0x31); // '1'
            Debug.Assert(_buffer[_currentPos++] == 0x31); // '1'

            var packetLength = BitConverter.ToInt32(_buffer, _currentPos);
            _currentPos += 4;
            if (packetLength == 0)
            {
                return null;
            }

            var packetType = (DvrPacketType)BitConverter.ToInt32(_buffer, _currentPos);
            _currentPos += packetLength;
            switch (packetType)
            {
                case DvrPacketType.CameraInfo:
                    return DvrCameraInfoPacket.FromData(_buffer, packetStart, packetLength);
                case DvrPacketType.DeviceInformation:
                    return DvrDeviceInformationPacket.FromData(_buffer, packetStart, packetLength);
                default:
                    return DvrUnsupportedPacket.FromData(_buffer, packetStart, packetLength);
            }
        }

        private void ReadMoreData()
        {
            _lastRead = _socket.Receive(_buffer);
            _currentPos = 0;
        }

        public void Close()
        {
            _socket?.Close();
        }
    }
}
