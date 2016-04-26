using System;
using System.Net.Sockets;
using System.Text;

namespace DvrSniffer
{
    public abstract class DvrPacket
    {
        private byte[] _data;

        public abstract DvrPacketType PacketType { get; }

        protected DvrPacket(int dataSize)
        {
            _data = new byte[dataSize + 8];
            _data[0] = (byte)'1';
            _data[1] = (byte)'1';
            _data[2] = (byte)'1';
            _data[3] = (byte)'1';
            Array.Copy(BitConverter.GetBytes(dataSize), 0, _data, 4, 4); // set packet size (less 4 byte '1111' and 4 byte size)
            Array.Copy(BitConverter.GetBytes((int)PacketType), 0, _data, 8, 4); // set packet type
        }

        /// <summary>
        /// Create a packet from received data.
        /// </summary>
        protected DvrPacket(byte[] data, int offset, int length)
            : this(length)
        {
            Array.Copy(data, offset, _data, 0, length);
        }

        public void Send(Socket socket)
        {
            socket.Send(_data, _data.Length, SocketFlags.None);
        }

        protected void ClearBytes(int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                _data[offset + i] = 0x00;
            }
        }

        protected void SetString(string str, int offset, int length = -1)
        {
            if (length < 0)
            {
                length = str.Length;
            }

            for (int i = 0; i < length; i++)
            {
                _data[offset + i] = (byte)str[i];
            }
        }

        protected void SetBytes(byte[] data, int offset)
        {
            Array.Copy(data, 0, _data, offset, data.Length);
        }

        protected void SetByte(byte value, int offset)
        {
            _data[offset] = value;
        }

        protected short GetShort(int offset)
        {
            return BitConverter.ToInt16(_data, offset);
        }

        protected string GetString(int offset)
        {
            var sb = new StringBuilder();
            while (_data[offset] != 0)
            {
                sb.Append((char)_data[offset++]);
            }

            return sb.ToString();
        }

        protected string GetHexString(int offset, int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append((((int)_data[offset + i])).ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
