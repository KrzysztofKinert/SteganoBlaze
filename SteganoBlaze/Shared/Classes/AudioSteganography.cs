using System.Text;
using SteganoBlaze.Shared.Classes.Types;
namespace SteganoBlaze.Shared.Classes
{
    public class AudioSteganography
    {
        public static byte[]? EncryptTextLinear(byte[] wav, string text)
        {
            WAV audio = new WAV(wav);
            uint value = 0;
            string pass = string.Format(audio.bitsPerSample.ToString());
            if (text.Length <= Math.Floor((double)(audio.samples / 8)))
            {
                int n = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    value = text[i];
                    for (int x = 0; x < 8; x++)
                    {
                        uint sampleValue = audio.GetSample(n);
                        sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                        audio.SetSample(n, sampleValue);
                        //audio.samples[m] = sampleValue;
                        n++;
                    }

                }
                value = 0;
                for (int x = 0; x < 8; x++)
                {
                    uint sampleValue = audio.GetSample(n);
                    sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                    audio.SetSample(n, sampleValue);
                    n++;
                }
                //audio.Save();
                return audio.data;
            }
            else
            {
                return null;
            }

        }

        public static string DecryptTextLinear(byte[] wav)
        {
            WAV audio = new WAV(wav);
            string text = string.Empty;
            int n = 0;
            uint value = 0;
            //string pass = string.Format(audio.bitsPerSample.ToString());
            do
            {
                value = 0;
                for (int x = 0; x < 8; x++)
                {
                    //uint sample = n;
                    //uint sampleValue = audio.samples[sample];
                    uint sampleValue = audio.GetSample(n);
                    value = value | ((sampleValue & 1) << x);
                    n++;
                }
                if (value != 0)
                    text += Convert.ToChar(value);
            } while (value != 0);
            return text;
        }

        public static byte[] EncryptFileLinear(byte[] wav, byte[] file)
        {
            WAV audio = new WAV(wav);
            uint value = 0;
            // int extraBytes = 2 + filename.Length + file.Length.ToString().Length;
            //OutputConsole.Write(string.Format("File size: {0} bytes", file.Length));
            if (file.Length <= Math.Floor((double)(audio.samples / 8)))
            {
                int n = 0;
                //Write file content
                //OutputConsole.Write("Writing file data...");
                for (int i = 0; i < file.Length; i++)
                {
                    value = file[i];
                    for (int x = 0; x < 8; x++)
                    {
                        uint sampleValue = audio.GetSample(n);
                        sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                        audio.SetSample(n, sampleValue);
                        n++;
                    }

                }
            }
            else
            {
                //OutputConsole.Write("Error");
                return Array.Empty<byte>();
            }
            //OutputConsole.Write("Finished embedding file");
            //OutputConsole.Write(string.Format("Used {0} samples", (file.Length + extraBytes) * 8));
            //audio.Save();
            return audio.data;
        }

        public static byte[]? Encode(byte[] wav, byte[] message, int bitsToUse)
        {
            WAV audio = new WAV(wav);
            uint value = 0;
            bool isEncodingDone = false;
            int byteIndex = 0;
            int byteValue = 0;
            int bitsEncoded = 8;
            int indexSample = bitsToUse;


            for (int i = 0; i < audio.data.Length; i++)
            {
                uint sample = audio.GetSample2(i);

                for (int n = 0; n < bitsToUse; n++)
                {
                    if (bitsEncoded == 8)
                    {
                        if (isEncodingDone == true)
                        {
                            if (indexSample != bitsToUse)
                            {
                                audio.SetSample2(i, sample);
                            }
                            return audio.data;
                        }
                        if (byteIndex >= message.Length)
                        {
                            byteValue = 0;
                            isEncodingDone = true;
                        }
                        else
                        {
                            byteValue = message[byteIndex];
                            byteIndex++;
                            bitsEncoded = 0;
                        }
                    }
                    if (isEncodingDone == false)
                    {
                        int bit = byteValue % 2;
                        byteValue /= 2;
                        sample = (uint)ChangeNthBit((int)sample, bit, indexSample - 1);

                        indexSample--;
                        bitsEncoded++;
                        if (indexSample == 0)
                        {
                            indexSample = bitsToUse;
                        }
                    }
                }
                audio.SetSample2(i, sample);
            }

            return null;
        }

        private static int ChangeNthBit(int value, int bit, int n)
        {
            return (value & ~(1 << n) | (bit << n));
        }
        //public static byte[] DecryptFileLinear(byte[] wav)
        //{
        //    try
        //    {
        //        WavFile audio = new WavFile(wav);
        //        string text = string.Empty;
        //        int n = 0;

        //        uint value = 0;
        //        //byte[] file = new byte[filesize];
        //        byte[] last7Bytes = new byte[7];
        //        List<byte> file = new List<byte>();
        //        for (int i = 0; i < wav.Length; i++)
        //        {
        //            value = 0;
        //            for (int x = 0; x < 8; x++)
        //            {
        //                uint sampleValue = audio.GetSample(n);
        //                value = value | ((sampleValue & 1) << x);
        //                n++;
        //            }
        //            file.Add((byte)value);

        //            Array.Copy(last7Bytes, 1, last7Bytes, 0, 6);
        //            last7Bytes[6] = Convert.ToByte(value);
        //            string last7Characters = UnicodeEncoding.Default.GetString(last7Bytes);
        //            if (last7Characters == "!#end#!")
        //            {
        //                return file.ToArray();
        //            }
        //        }
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}

        public static int CheckAudio(WAV audio)
        {
            for (int bitsToUse = 1; bitsToUse <= audio.bitsPerSample; bitsToUse++)
            {
                List<byte> decodedBytes = new List<byte>();

                int decodedBits = 0;
                uint byteValue = 0;

                int indexSample = bitsToUse;

                for (int i = 0; decodedBytes.Count < 9; i++)
                {
                    uint sample = audio.GetSample2(i);

                    for (int n = 0; n < bitsToUse; n++)
                    {
                        int bitSign = (int)(sample / ((int)Math.Pow(2, indexSample - 1)) % 2);
                        byteValue += (uint)((int)Math.Pow(2, 7 - decodedBits) * bitSign);

                        indexSample--;
                        decodedBits++;
                        if (indexSample == 0)
                        {
                            indexSample = bitsToUse;
                        }


                        if (decodedBits == 8)
                        {
                            byteValue = ReverseBits(byteValue);
                            decodedBytes.Add(Convert.ToByte(byteValue));

                            string decodedString = UnicodeEncoding.Default.GetString(decodedBytes.ToArray());
                            if (decodedString == "!encoded!")
                            {
                                return bitsToUse;
                            }

                            decodedBits = 0;
                            byteValue = 0;

                        }
                    }
                }
            }
            return 0;
        }


        public static byte[] Decode(byte[] carrier, int bitsToUse)
        {
            WAV audio = new WAV(carrier);
            List<byte> recoveredFile = new List<byte>();

            int decodedBits = 0;
            uint byteValue = 0;

            int indexSample = bitsToUse;

            byte[] last7Bytes = new byte[7];

            for (int i = 0; i < carrier.Length; i++)
            {
                uint sample = audio.GetSample2(i);

                for (int n = 0; n < bitsToUse; n++)
                {
                    int bitSign = (int)(sample / ((int)Math.Pow(2, indexSample - 1)) % 2);
                    byteValue += (uint)((int)Math.Pow(2, 7 - decodedBits) * bitSign);

                    indexSample--;
                    decodedBits++;
                    if (indexSample == 0)
                    {
                        indexSample = bitsToUse;
                    }


                    if (decodedBits == 8)
                    {
                        byteValue = ReverseBits(byteValue);
                        recoveredFile.Add(Convert.ToByte(byteValue));

                        Array.Copy(last7Bytes, 1, last7Bytes, 0, 6);
                        last7Bytes[6] = Convert.ToByte(byteValue);
                        string last7Characters = UnicodeEncoding.Default.GetString(last7Bytes);
                        if (last7Characters == "!#end#!")
                        {
                            return recoveredFile.ToArray();
                        }

                        decodedBits = 0;
                        byteValue = 0;

                    }
                }
            }
            return recoveredFile.ToArray();
        }

        private static uint ReverseBits(uint value)
        {
            uint result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + value % 2;
                value /= 2;
            }

            return result;
        }

    }
}
