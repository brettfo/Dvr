using System;

namespace DvrSniffer
{
    public class DvrUnsupportedPacket : DvrPacket
    {
        private DvrPacketType _packetType;

        public override DvrPacketType PacketType => _packetType;

        internal DvrUnsupportedPacket(DvrPacketType packetType)
        {
            _packetType = packetType;
        }
    }
}
