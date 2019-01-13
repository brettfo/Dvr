using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DvrSniffer
{
    public abstract class DvrPacket
    {
        private byte[] _data;

        public abstract DvrPacketType PacketType { get; }

        protected DvrPacket()
        {
        }

        /// <summary>
        /// Create a packet from received data.
        /// </summary>
        protected DvrPacket(int length)
        {
            _data = new byte[length];
            _data[0] = (byte)'1';
            _data[1] = (byte)'1';
            _data[2] = (byte)'1';
            _data[3] = (byte)'1';
            Array.Copy(BitConverter.GetBytes(length - 8), 0, _data, 4, 4); // set packet size (less 4 byte '1111' and 4 byte size)
            Array.Copy(BitConverter.GetBytes((int)PacketType), 0, _data, 8, 4); // set packet type
        }

        public Task<int> SendAsync(Socket socket)
        {
            return socket.SendAsync(_data, SocketFlags.None);
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

        protected void SetShort(short value, int offset)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _data, offset, 2);
        }

        protected static short GetShort(byte[] data, int offset)
        {
            return BitConverter.ToInt16(data, offset);
        }

        protected string GetString(int offset)
        {
            return GetString(_data, offset);
        }

        protected static string GetString(byte[] data, int offset)
        {
            var sb = new StringBuilder();
            while (data[offset] != 0)
            {
                sb.Append((char)data[offset++]);
            }

            return sb.ToString();
        }

        protected static string GetHexString(byte[] data, int offset, int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append((((int)data[offset + i])).ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
