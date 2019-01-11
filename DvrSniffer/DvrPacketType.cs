namespace DvrSniffer
{
    public enum DvrPacketType : uint
    {
        Login = 0x00000101,
        RequestVideo = 0x00000201,
        DeviceInformation = 0x00010001,
        Unknown1 = 0x09000001,
        Unknown2 = 0x09000002,
        Unknown3 = 0x09000003,
        Unknown4 = 0x09000004,
        CameraInfo = 0x09000008,
        VideoData = 0x0A000001
    }
}
