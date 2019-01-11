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

        public string SerialNumber { get; }

        public string DeviceName { get; }

        public string FirmwareVersion { get; }

        public string KernelVersion { get; }

        public string HardwareVersion { get; }

        public string MCUVersion { get; }

        private DvrDeviceInformationPacket(byte[] data)
        {
            SerialNumber = GetHexString(data, SerialNumberOffset, 6);
            DeviceName = GetString(data, DeviceNameOffset);
            FirmwareVersion = GetString(data, FirmwareVersionOffset);
            KernelVersion = GetString(data, KernelVersionOffset);
            HardwareVersion = GetString(data, HardwareVersionOffset);
            MCUVersion = GetString(data, MCUVersionOffset);
        }

        internal static DvrDeviceInformationPacket FromData(byte[] data)
        {
            return new DvrDeviceInformationPacket(data);
        }
    }
}
