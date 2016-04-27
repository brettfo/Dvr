using System.Collections.Generic;

namespace DvrSniffer
{
    public class DvrCameraInfoPacket : DvrPacket
    {
        private List<string> _cameraNames = new List<string>();

        public override DvrPacketType PacketType => DvrPacketType.CameraInfo;

        public IEnumerable<string> CameraNames { get; }

        private DvrCameraInfoPacket(IEnumerable<string> cameraNames)
        {
            CameraNames = cameraNames;
        }

        internal static DvrCameraInfoPacket FromData(byte[] data, int offset)
        {
            var cameraNames = new List<string>();
            // TODO: where is the camera count in the raw data?
            for (int i = 0; i < 8; i++)
            {
                var off = i * 0x28 + 0x14 + offset;
                var cameraNumber = GetShort(data, off + 0x04);
                var cameraName = GetString(data, off + 0x08);
                cameraNames.Insert(cameraNumber, cameraName);
            }

            return new DvrCameraInfoPacket(cameraNames);
        }
    }
}
