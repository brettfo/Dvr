using System.Collections.Generic;

namespace DvrSniffer
{
    public class DvrCameraInfoPacket : DvrPacket
    {
        private List<string> _cameraNames = new List<string>();

        public override DvrPacketType PacketType => DvrPacketType.CameraInfo;

        public IEnumerable<string> CameraNames => _cameraNames;

        private DvrCameraInfoPacket(byte[] data, int offset, int length)
            : base(data, offset, length)
        {
        }

        internal static DvrCameraInfoPacket FromData(byte[] data, int offset, int length)
        {
            var info = new DvrCameraInfoPacket(data, offset, length);
            // TODO: where is the camera count in the raw data?
            for (int i = 0; i < 8; i++)
            {
                var off = i * 0x28 + 0x14;
                var cameraNumber = info.GetShort(off + 0x04);
                var cameraName = info.GetString(off + 0x08);
                info._cameraNames.Insert(cameraNumber, cameraName);
            }

            return info;
        }
    }
}
