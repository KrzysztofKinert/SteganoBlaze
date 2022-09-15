using System.Text;
namespace SteganoBlaze.Shared.Classes
{
    public class WAV : File
    {
        public int bitsPerSample { get; private set; }

        int fmtChunkIndex;
        int dataChunkIndex;

        int channels;
        int sampleRate;
        int samples;

        public WAV(File file)
        {
            byteData = file.byteData;
            base64Data = file.base64Data;
            contentType = file.contentType;
            fileName = file.fileName;
            fileSize = file.fileSize;
            ParseWAV();
        }

        public byte[] GetData()
        {
            return byteData;
        }
        public int GetSampleCount()
        {
            return samples * channels;
        }
        public string GetDuration()
        {
            double durationInSeconds = (double)samples / sampleRate;
            TimeSpan duration = TimeSpan.FromSeconds(durationInSeconds);
            return string.Format("{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);
        }
        public uint GetSample(int n)
        {
            uint sample;
            int sampleIndex = n * bitsPerSample / 8;
            switch (bitsPerSample)
            {
                case 8:
                    sample = byteData[dataChunkIndex + sampleIndex];
                    break;
                case 16:
                default:
                    sample = byteData[dataChunkIndex + sampleIndex];
                    sample += (uint)(byteData[dataChunkIndex + sampleIndex + 1] << 8);
                    break;
                case 24:
                    sample = byteData[dataChunkIndex + sampleIndex];
                    sample += (uint)(byteData[dataChunkIndex + sampleIndex + 1] << 8);
                    sample += (uint)(byteData[dataChunkIndex + sampleIndex + 2] << 16);
                    break;
                case 32:
                    sample = byteData[dataChunkIndex + sampleIndex];
                    sample += (uint)(byteData[dataChunkIndex + sampleIndex + 1] << 8);
                    sample += (uint)(byteData[dataChunkIndex + sampleIndex + 2] << 16);
                    sample += (uint)(byteData[dataChunkIndex + sampleIndex + 3] << 24);
                    break;
            }
            return sample;
        }
        public void SetSample(int n, uint sample)
        {
            int sampleIndex = n * bitsPerSample / 8;
            switch (bitsPerSample)
            {
                case 8:
                    byteData[dataChunkIndex + sampleIndex] = (byte)sample;
                    break;
                case 16:
                default:
                    byteData[dataChunkIndex + sampleIndex] = (byte)sample;
                    byteData[dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    break;
                case 24:
                    byteData[dataChunkIndex + sampleIndex] = (byte)sample;
                    byteData[dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    byteData[dataChunkIndex + sampleIndex + 2] = (byte)(sample >> 16);
                    break;
                case 32:
                    byteData[dataChunkIndex + sampleIndex] = (byte)sample;
                    byteData[dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    byteData[dataChunkIndex + sampleIndex + 2] = (byte)(sample >> 16);
                    byteData[dataChunkIndex + sampleIndex + 3] = (byte)(sample >> 24);
                    break;
            }
        }

        public WAV Clone()
        {
            WAV clone = this;
            clone.byteData = (byte[])byteData.Clone();
            return clone;
        }
        void ParseWAV()
        {
            fmtChunkIndex = FindChunkIndex("fmt ");

            channels = BitConverter.ToInt16(byteData, fmtChunkIndex + 2);
            sampleRate = BitConverter.ToInt32(byteData, fmtChunkIndex + 4);
            bitsPerSample = BitConverter.ToInt16(byteData, fmtChunkIndex + 14);

            dataChunkIndex = FindChunkIndex("data");

            samples = BitConverter.ToInt32(byteData, dataChunkIndex - 4) / (channels * bitsPerSample / 8);

            bool validBitsPerSample = new List<int> { 8, 16, 24, 32 }.Contains(bitsPerSample);
            bool validChannels = new List<int> { 1, 2 }.Contains(channels);

            if (validBitsPerSample is false || validChannels is false)
                throw new Exception();
        }
        int FindChunkIndex(string chunkID)
        {
            byte[] chunkHeader = new byte[4];
            int i = 0;
            do
            {
                Array.Copy(byteData, i, chunkHeader, 0, 4);
                i++;
            }
            while (Encoding.UTF8.GetString(chunkHeader) != chunkID);
            return i - 1 + 8;
        }
    }
}
