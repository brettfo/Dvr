namespace DvrSniffer
{
    public class DvrRequestVideoDataPacket : DvrPacket
    {
        public override DvrPacketType PacketType => DvrPacketType.RequestVideo;

        public DvrRequestVideoDataPacket(short cameraNumber)
            : base(60)
        {
            // unknown bytes
            SetByte(0x24, 20);
            SetByte(0x01, 36);

            // TODO: where to set the camera number?
            SetShort(cameraNumber, 18);
        }
    }
}
