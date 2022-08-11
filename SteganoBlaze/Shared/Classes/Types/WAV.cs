using System.Text;

namespace SteganoBlaze.Shared.Classes.Types
{
    public class WAV
    {
        public byte[] data;

        public readonly int fmtChunkIndex;
        public readonly int dataChunkIndex;
        public int channels { get; private set; }
        public int sampleRate { get; private set; }
        public int bitsPerSample { get; private set; }
        public int samples { get; private set; }
        public long bytesAvailable { get; private set; }

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
            while (Encoding.Default.GetString(chunkHeader) != "fmt ");
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
            while (Encoding.Default.GetString(chunkHeader) != "data");
            dataChunkIndex = fmtChunkIndex + i - 1 + 8;

            if (bitsPerSample == 8 || bitsPerSample == 16 || bitsPerSample == 24 || bitsPerSample == 32)
            {
                samples = BitConverter.ToInt32(data, dataChunkIndex - 4) / (channels * bitsPerSample / 8);
                //totalSamples = (data.Length - 44) / (channels * bitsPerSample / 8);
                /*int i = 0;
                for (int n = 0; n < totalSamples; n++)
                {
                    switch (bitsPerSample)
                    {
                        case 8:
                            samples[n] = data[start + i];
                            break;
                        case 16:
                        default:
                            samples[n] = BitConverter.ToUInt16(data, start + i);
                            break;
                        case 24:
                            samples[n] = BitConverter.ToUInt32(data, start + i) & 0xFFFFFF;
                            break;
                        case 32:
                            samples[n] = BitConverter.ToUInt32(data, start + i);
                            break;
                    }
                    i += (int)(bitsPerSample / 8);
                }*/
                bytesAvailable = (long)Math.Floor((double)(samples / 8));
            }
            else
            {
                this.data = Array.Empty<byte>();
            }
        }

        public string GetDuration()
        {
            //int durationInSeconds = (totalSamples / (sampleRate * channels * bitsPerSample / 8));
            double durationInSeconds = (double)samples / sampleRate;
            TimeSpan duration = TimeSpan.FromSeconds(durationInSeconds);
            return string.Format("{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);
        }
        public uint GetSample(int n)
        {
            uint sample;
            int i = n * (int)(bitsPerSample / 8);
            switch (bitsPerSample)
            {
                case 8:
                    sample = data[dataChunkIndex + i];
                    break;
                case 16:
                default:
                    sample = BitConverter.ToUInt16(data, dataChunkIndex + i);
                    break;
                case 24:
                    sample = BitConverter.ToUInt32(data, dataChunkIndex + i) & 0xFFFFFF;
                    break;
                case 32:
                    sample = BitConverter.ToUInt32(data, dataChunkIndex + i);
                    break;
            }
            return sample;
        }
        public uint GetSample2(int n)
        {
            uint sample;
            int i = n * (int)(bitsPerSample / 8);
            switch (bitsPerSample)
            {
                case 8:
                    sample = data[dataChunkIndex + i];
                    break;
                case 16:
                default:
                    sample = data[dataChunkIndex + i];
                    sample += (uint)(data[dataChunkIndex + i + 1] << 8);
                    break;
                case 24:
                    sample = data[dataChunkIndex + i];
                    sample += (uint)(data[dataChunkIndex + i + 1] << 8);
                    sample += (uint)(data[dataChunkIndex + i + 2] << 16);
                    break;
                case 32:
                    sample = data[dataChunkIndex + i];
                    sample += (uint)(data[dataChunkIndex + i + 1] << 8);
                    sample += (uint)(data[dataChunkIndex + i + 2] << 16);
                    sample += (uint)(data[dataChunkIndex + i + 3] << 24);
                    break;
            }
            return sample;
        }
        public void SetSample(int n, uint sample)
        {
            int i = n * (int)(bitsPerSample / 8);
            switch (bitsPerSample)
            {
                case 8:
                    data[dataChunkIndex + i] = (byte)sample;
                    break;
                case 16:
                default:
                    data[dataChunkIndex + i] = (byte)(sample & 0xFF);
                    data[dataChunkIndex + i + 1] = (byte)((sample >> 8) & 0xFF);
                    break;
                case 24:
                    data[dataChunkIndex + i] = (byte)(sample & 0xFF);
                    data[dataChunkIndex + i + 1] = (byte)((sample >> 8) & 0xFF);
                    data[dataChunkIndex + i + 2] = (byte)((sample >> 16) & 0xFF);
                    break;
                case 32:
                    data[dataChunkIndex + i] = (byte)(sample & 0xFF);
                    data[dataChunkIndex + i + 1] = (byte)((sample >> 8) & 0xFF);
                    data[dataChunkIndex + i + 2] = (byte)((sample >> 16) & 0xFF);
                    data[dataChunkIndex + i + 3] = (byte)((sample >> 24) & 0xFF);
                    break;
            }
        }
        public void SetSample2(int n, uint sample)
        {
            int i = n * (int)(bitsPerSample / 8);
            switch (bitsPerSample)
            {
                case 8:
                    data[dataChunkIndex + i] = (byte)sample;
                    break;
                case 16:
                default:
                    data[dataChunkIndex + i] = (byte)(sample);
                    data[dataChunkIndex + i + 1] = (byte)(sample >> 8);
                    break;
                case 24:
                    data[dataChunkIndex + i] = (byte)(sample);
                    data[dataChunkIndex + i + 1] = (byte)(sample >> 8);
                    data[dataChunkIndex + i + 2] = (byte)(sample >> 16);
                    break;
                case 32:
                    data[dataChunkIndex + i] = (byte)(sample);
                    data[dataChunkIndex + i + 1] = (byte)(sample >> 8);
                    data[dataChunkIndex + i + 2] = (byte)(sample >> 16);
                    data[dataChunkIndex + i + 3] = (byte)(sample >> 24);
                    break;
            }
        }
        //public void Save()
        //{
        //    if (bitsPerSample == 8 || bitsPerSample == 16 || bitsPerSample == 24 || bitsPerSample == 32)
        //    {
        //        int i = 0;
        //        for (int n = 0; n < totalSamples; n++)
        //        {
        //            switch (bitsPerSample)
        //            {
        //                case 8:
        //                    data[start + i] = (byte)samples[n];
        //                    break;
        //                case 16:
        //                default:
        //                    data[start + i] = (byte)(samples[n] & 0xFF);
        //                    data[start + i + 1] = (byte)((samples[n] >> 8) & 0xFF);
        //                    break;
        //                case 24:
        //                    data[start + i] = (byte)(samples[n] & 0xFF);
        //                    data[start + i + 1] = (byte)((samples[n] >> 8) & 0xFF);
        //                    data[start + i + 2] = (byte)((samples[n] >> 16) & 0xFF);
        //                    break;
        //                case 32:
        //                    data[start + i] = (byte)(samples[n] & 0xFF);
        //                    data[start + i + 1] = (byte)((samples[n] >> 8) & 0xFF);
        //                    data[start + i + 2] = (byte)((samples[n] >> 16) & 0xFF);
        //                    data[start + i + 3] = (byte)((samples[n] >> 24) & 0xFF);
        //                    break;
        //            }
        //            i += (int)(bitsPerSample / 8);
        //        }
        //    }
        //}
    }
}
