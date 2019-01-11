using System;

namespace DvrSniffer
{
    public enum FrameType : byte
    {
        Unknown = 0x00,
        IFrame = 0x65,
        SPSFrame = 0x67,
        PPSFrame = 0x68,
        PSFrame = 0x61,
    }

    public class DvrVideoDataPacket : DvrPacket
    {
        public override DvrPacketType PacketType => DvrPacketType.VideoData;

        public FrameType FrameType { get; }
        public byte[] Data { get; }

        private DvrVideoDataPacket(FrameType frameType, byte[] data, int offset, int length)
        {
            FrameType = frameType;
            Data = new byte[length];
            Array.Copy(data, offset, Data, 0, length);
        }

        public static DvrVideoDataPacket FromData(byte[] data, int offset, int length)
        {
            //Console.WriteLine("new video data packet");
            // TODO: the output contains some h.264 values:
            // "00 00 00 01 65" iframe
            // "00 00 00 01 67" SPS frame
            // "00 00 00 01 68" PPS frame
            // "00 00 00 01 61" PS frame
            //var bytesToTrim = 24; // 8 for the 'ff ff ff ff 00 00 00 00' and 24 for the next line that seems to have a pattern

            // look for 00 00 00 01 6x
            for (int i = 0; i < data.Length - 5; i++)
            {
                if (data[i] == 0x00 &&
                    data[i + 1] == 0x00 &&
                    data[i + 2] == 0x00 &&
                    data[i + 3] == 0x01 &&
                    data[i + 4] >= 0x60 &&
                    data[i + 4] <= 0x6F)
                {
                    if (Enum.IsDefined(typeof(FrameType), data[i + 4]))
                    {
                        var frameType = (FrameType)data[i + 4];
                        Console.WriteLine($"Found frame type {frameType} at offset {i}.");
                    }
                }
            }

            // look for 00 00 00 01 6x
            for (int i = 0; i < data.Length - 5; i++)
            {
                if (data[i] == 0x00 &&
                    data[i + 1] == 0x00 &&
                    data[i + 2] == 0x00 &&
                    data[i + 3] == 0x01 &&
                    data[i + 4] >= 0x60 &&
                    data[i + 4] <= 0x6F)
                {
                    var frameType = FrameType.Unknown;
                    if (Enum.IsDefined(typeof(FrameType), data[i + 4]))
                    {
                        frameType = (FrameType)data[i + 4];
                    }

                    return new DvrVideoDataPacket(frameType, data, i, data.Length - i);
                }
            }

            return null;
        }
    }
}
