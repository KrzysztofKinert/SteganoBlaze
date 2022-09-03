using System.Text;

namespace SteganoBlaze.Shared.Classes.Types
{
    public class WAV
    {
        private byte[] data;
        public int bitsPerSample { get; private set; }

        private int fmtChunkIndex;
        private int dataChunkIndex;

        private int channels;
        private int sampleRate;
        private int samples;

        public WAV(byte[] data)
        {
            this.data = data;

            byte[] chunkHeader = new byte[4];

            int i = 0;
            do
            {
                Array.Copy(data, i, chunkHeader, 0, 4);
                i++;
            }
            while (Encoding.UTF8.GetString(chunkHeader) != "fmt ");
            fmtChunkIndex = i - 1 + 8;

            channels = BitConverter.ToInt16(data, fmtChunkIndex + 2);
            sampleRate = BitConverter.ToInt32(data, fmtChunkIndex + 4);
            bitsPerSample = BitConverter.ToInt16(data, fmtChunkIndex + 14);

            i = 0;
            do
            {
                Array.Copy(data, fmtChunkIndex + i, chunkHeader, 0, 4);
                i++;
            }
            while (Encoding.UTF8.GetString(chunkHeader) != "data");
            dataChunkIndex = fmtChunkIndex + i - 1 + 8;

            samples = BitConverter.ToInt32(data, dataChunkIndex - 4) / (channels * bitsPerSample / 8);

            bool validBitsPerSample = new List<int> { 8, 16, 24, 32 }.Contains(bitsPerSample);
            bool validChannels = new List<int> { 1, 2 }.Contains(channels);

            if(validBitsPerSample is false || validChannels is false)
                throw new Exception();
        }
        public byte[] GetData()
        {
            return data;
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
                    sample = data[dataChunkIndex + sampleIndex];
                    break;
                case 16:
                default:
                    sample = data[dataChunkIndex + sampleIndex];
                    sample += (uint)(data[dataChunkIndex + sampleIndex + 1] << 8);
                    break;
                case 24:
                    sample = data[dataChunkIndex + sampleIndex];
                    sample += (uint)(data[dataChunkIndex + sampleIndex + 1] << 8);
                    sample += (uint)(data[dataChunkIndex + sampleIndex + 2] << 16);
                    break;
                case 32:
                    sample = data[dataChunkIndex + sampleIndex];
                    sample += (uint)(data[dataChunkIndex + sampleIndex + 1] << 8);
                    sample += (uint)(data[dataChunkIndex + sampleIndex + 2] << 16);
                    sample += (uint)(data[dataChunkIndex + sampleIndex + 3] << 24);
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
                    data[dataChunkIndex + sampleIndex] = (byte)sample;
                    break;
                case 16:
                default:
                    data[dataChunkIndex + sampleIndex] = (byte)(sample);
                    data[dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    break;
                case 24:
                    data[dataChunkIndex + sampleIndex] = (byte)(sample);
                    data[dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    data[dataChunkIndex + sampleIndex + 2] = (byte)(sample >> 16);
                    break;
                case 32:
                    data[dataChunkIndex + sampleIndex] = (byte)(sample);
                    data[dataChunkIndex + sampleIndex + 1] = (byte)(sample >> 8);
                    data[dataChunkIndex + sampleIndex + 2] = (byte)(sample >> 16);
                    data[dataChunkIndex + sampleIndex + 3] = (byte)(sample >> 24);
                    break;
            }
        }
    }
}
