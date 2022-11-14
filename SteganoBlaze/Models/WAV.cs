using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("SteganoBlaze.Tests")]

namespace SteganoBlaze.Models
{ 
    public class Wav : File
    {
        public int BitsPerSample { get; private set; }

        int _fmtChunkIndex;
        int _dataChunkIndex;

        internal int Channels;
        internal int SampleRate;
        internal int SampleCount;

        public Wav(File file)
        {
            ByteData = file.ByteData;
            Base64Data = file.Base64Data;
            ContentType = file.ContentType;
            FileName = file.FileName;
            FileSize = file.FileSize;
            ParseWav();
        }

        public byte[] GetData()
        {
            return ByteData;
        }
        public int GetTotalSampleCount()
        {
            return SampleCount * Channels;
        }
        public string GetDuration()
        {
            var durationInSeconds = (double)SampleCount / SampleRate;
            var duration = TimeSpan.FromSeconds(durationInSeconds);
            return string.Format("{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);
        }
        public uint GetSample(int n)
        {
            uint sample = 0;
            var sampleIndex = n * BitsPerSample / 8;
            switch (BitsPerSample)
            {
                case 8:
                    sample = ByteData[_dataChunkIndex + sampleIndex];
                    break;
                case 16:
                    sample = ByteData[_dataChunkIndex + sampleIndex];
                    sample += (uint)(ByteData[_dataChunkIndex + sampleIndex + 1] << 8);
                    break;
                case 24:
                    sample = ByteData[_dataChunkIndex + sampleIndex];
                    sample += (uint)(ByteData[_dataChunkIndex + sampleIndex + 1] << 8);
                    sample += (uint)(ByteData[_dataChunkIndex + sampleIndex + 2] << 16);
                    break;
                case 32:
                    sample = ByteData[_dataChunkIndex + sampleIndex];
                    sample += (uint)(ByteData[_dataChunkIndex + sampleIndex + 1] << 8);
                    sample += (uint)(ByteData[_dataChunkIndex + sampleIndex + 2] << 16);
                    sample += (uint)(ByteData[_dataChunkIndex + sampleIndex + 3] << 24);
                    break;
            }
            return sample;
        }
        public void SetSample(int n, uint sample)
        {
            var sampleIndex = n * BitsPerSample / 8;
            switch (BitsPerSample)
            {
                case 8:
                    ByteData[_dataChunkIndex + sampleIndex] = (byte)sample;
                    break;
                case 16:
                    ByteData[_dataChunkIndex + sampleIndex] = (byte)sample;
                    ByteData[_dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    break;
                case 24:
                    ByteData[_dataChunkIndex + sampleIndex] = (byte)sample;
                    ByteData[_dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    ByteData[_dataChunkIndex + sampleIndex + 2] = (byte)(sample >> 16);
                    break;
                case 32:
                    ByteData[_dataChunkIndex + sampleIndex] = (byte)sample;
                    ByteData[_dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    ByteData[_dataChunkIndex + sampleIndex + 2] = (byte)(sample >> 16);
                    ByteData[_dataChunkIndex + sampleIndex + 3] = (byte)(sample >> 24);
                    break;
            }
        }

        public Wav Clone()
        {
            var clone = this;
            clone.ByteData = (byte[])ByteData.Clone();
            return clone;
        }
        internal void ParseWav()
        {
            _fmtChunkIndex = FindChunkIndex("fmt ");

            Channels = BitConverter.ToInt16(ByteData, _fmtChunkIndex + 2);
            SampleRate = BitConverter.ToInt32(ByteData, _fmtChunkIndex + 4);
            BitsPerSample = BitConverter.ToInt16(ByteData, _fmtChunkIndex + 14);

            _dataChunkIndex = FindChunkIndex("data");

            SampleCount = BitConverter.ToInt32(ByteData, _dataChunkIndex - 4) / (Channels * BitsPerSample / 8);

            var validBitsPerSample = new List<int> { 8, 16, 24, 32 }.Contains(BitsPerSample);
            var validChannels = new List<int> { 1, 2 }.Contains(Channels);

            if (validBitsPerSample is false || validChannels is false)
                throw new Exception();
        }
        internal int FindChunkIndex(string chunkId)
        {
            var chunkHeader = new byte[4];
            var i = 0;
            do
            {
                Array.Copy(ByteData, i, chunkHeader, 0, 4);
                i++;
            }
            while (Encoding.UTF8.GetString(chunkHeader) != chunkId);
            return i - 1 + 8;
        }
    }
}
