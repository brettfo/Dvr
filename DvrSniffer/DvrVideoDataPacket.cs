using System;
using System.Collections.Generic;

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

    public class VideoDataFrame
    {
        public short CameraNumber { get; }
        public FrameType FrameType { get; }
        public byte[] Data { get; }

        public VideoDataFrame(short cameraNumber, FrameType frameType, byte[] data)
        {
            CameraNumber = cameraNumber;
            FrameType = frameType;
            Data = data;
        }
    }

    public class DvrVideoDataPacket : DvrPacket
    {
        public override DvrPacketType PacketType => DvrPacketType.VideoData;

        public byte[] Data { get; }

        public List<VideoDataFrame> VideoDataFrames { get; }

        private DvrVideoDataPacket(byte[] data, List<VideoDataFrame> videoDataFrames)
        {
            Data = data;
            VideoDataFrames = videoDataFrames;
        }

        public static DvrVideoDataPacket FromData(byte[] data)
        {
            var lastDataFrameStart = -1;
            var dataFrameOffsets = new List<(int, FrameType)>();
            while (TryFindNextVideoDataFrame(data, lastDataFrameStart + 1, out var dataFrameStart, out var frameType))
            {
                dataFrameOffsets.Add((dataFrameStart, frameType));
                lastDataFrameStart = dataFrameStart;
            }

            var dataFrames = new List<VideoDataFrame>();
            for (int i = 0; i < dataFrameOffsets.Count; i++)
            {
                (var frameStart, var frameType) = dataFrameOffsets[i];
                var nextFrameStart = i == dataFrameOffsets.Count - 1
                    ? data.Length
                    : dataFrameOffsets[i + 1].Item1;
                var frameLength = nextFrameStart - frameStart;
                var frameData = new byte[frameLength];
                Array.Copy(data, frameStart, frameData, 0, frameLength);
                var dataFrame = new VideoDataFrame(-1, frameType, frameData);
                dataFrames.Add(dataFrame);
            }

            return new DvrVideoDataPacket(data, dataFrames);
        }

        private static bool TryFindNextVideoDataFrame(byte[] data, int startOffset, out int nextDataFrameStart, out FrameType frameType)
        {
            nextDataFrameStart = default(int);
            frameType = default(FrameType);
            for (int i = startOffset; i < data.Length - 5; i++)
            {
                if (data[i] == 0x00 &&
                    data[i + 1] == 0x00 &&
                    data[i + 2] == 0x00 &&
                    data[i + 3] == 0x01 &&
                    data[i + 4] >= 0x60 &&
                    data[i + 4] <= 0x6F)
                {
                    nextDataFrameStart = i;
                    frameType = (FrameType)data[i + 4];
                    return true;
                }
            }

            return false;
        }
    }
}
