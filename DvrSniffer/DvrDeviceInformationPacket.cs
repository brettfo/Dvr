namespace DvrSniffer
{
    public class DvrDeviceInformationPacket : DvrPacket
    {
        private const int SerialNumberOffset = 0x94;
        private const int DeviceNameOffset = 0xA4;
        private const int FirmwareVersionOffset = 0xC8;
        private const int KernelVersionOffset = 0xEC;
        private const int HardwareVersionOffset = 0x130;
        private const int MCUVersionOffset = 0x150;

        public override DvrPacketType PacketType => DvrPacketType.DeviceInformation;

        public string SerialNumber => GetHexString(SerialNumberOffset, 6);

        public string DeviceName => GetString(DeviceNameOffset);

        public string FirmwareVersion => GetString(FirmwareVersionOffset);

        public string KernelVersion => GetString(KernelVersionOffset);

        public string HardwareVersion => GetString(HardwareVersionOffset);

        public string MCUVersion => GetString(MCUVersionOffset);

        private DvrDeviceInformationPacket(byte[] data, int offset, int length)
            : base(data, offset, length)
        {
        }

        internal static DvrDeviceInformationPacket FromData(byte[] data, int offset, int length)
        {
            return new DvrDeviceInformationPacket(data, offset, length);
        }
    }
}
