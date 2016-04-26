using System;

namespace DvrSniffer
{
    public class DvrUnsupportedPacket : DvrPacket
    {
        private DvrPacketType _packetType;

        public override DvrPacketType PacketType => _packetType;

        private DvrUnsupportedPacket(DvrPacketType packetType, byte[] data, int offset, int length)
            : base(data, offset, length)
        {
            _packetType = packetType;
        }

        internal static DvrUnsupportedPacket FromData(byte[] data, int offset, int length)
        {
            var packetType = (DvrPacketType)BitConverter.ToInt32(data, offset + 8);
            return new DvrUnsupportedPacket(packetType, data, offset, length);
        }
    }
}
