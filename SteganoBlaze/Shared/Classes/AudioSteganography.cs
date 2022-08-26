using SteganoBlaze.Shared.Classes.Types;
using System.Text;
using System.Threading.Channels;
using static MudBlazor.CategoryTypes;
using static SteganoBlaze.Shared.Classes.ImageSteganography;

namespace SteganoBlaze.Shared.Classes
{
	public abstract class AudioSteganography
	{
        public enum SampleOrder
        {
            Sequential,
            Random
        }

        protected byte[] pixelData = Array.Empty<byte>();
        protected WAV carrier;
        protected int sampleIndex;
        protected int bitsLeft;
        protected int bitsToUse;

        protected SampleOrder sampleOrder;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedSamples;

        protected void FirstSample()
        {
            bitsLeft = bitsToUse;

            switch (sampleOrder)
            {
                case SampleOrder.Sequential:
                    sampleIndex = 0;
                    break;
                case SampleOrder.Random:
                    generator = new Random(randomSeed);
                    usedSamples = new HashSet<int>();
                    //sampleIndex = generator.Next(pixelData.Length / 4) * 4; ////AAAAAAAAAAAAA
                    sampleIndex = generator.Next(carrier.samples);
                    usedSamples.Add(sampleIndex);
                    break;
            }
        }
        protected void NextSample()
        {
            bitsLeft = bitsToUse;

            switch (sampleOrder)
            {
                case SampleOrder.Sequential:
                    sampleIndex++;
                    break;
                case SampleOrder.Random:
                    while (usedSamples!.Contains(sampleIndex))
                        sampleIndex = generator!.Next(carrier.samples);
                    //sampleIndex = generator!.Next(pixelData.Length / 4) * 4; //AAAAAAAAAAAAAAAA
                    usedSamples.Add(sampleIndex);
                    break;
            }
        }
    }
	public class AudioEncoder : AudioSteganography
	{
        public AudioEncoder(WAV carrier, int bits, bool randomEncodingEnabled)
        {
            this.carrier = carrier;
            bitsToUse = bits;
            randomSeed = carrier.samples;

            if (randomEncodingEnabled)
                sampleOrder = SampleOrder.Random;
            else
                sampleOrder = SampleOrder.Sequential;

            FirstSample();
        }
        public WAV Encode(Message message)
        {
            foreach (byte headerByte in message.header)
                EncodeByte(headerByte);

            foreach (byte metadataByte in message.metadata)
                EncodeByte(metadataByte);

            foreach (byte fileByte in message.file)
                EncodeByte(fileByte);

            return carrier;
        }
        private void EncodeByte(byte byteValue)
        {
            int encodedBits = 0;
            while (encodedBits < 8)
            {
                while (bitsLeft > 0)
                {
                    int bitSign = byteValue % 2;
                    byteValue /= 2;

                    uint sample = carrier.GetSample2(sampleIndex);
                    sample = (uint)ChangeNthBit((int)sample, bitSign, bitsLeft - 1);
                    carrier.SetSample2(sampleIndex, sample);

                    //var mask = 1 << (bitsLeft - 1);
                    //if (bitSign == 1)
                    //    sample |= (byte)mask;
                    //else
                    //    sample &= (byte)~mask;

                    bitsLeft--;
                    encodedBits++;

                    if (encodedBits == 8)
                        return;
                }
                NextSample();
            }
        }
        private static int ChangeNthBit(int value, int bit, int n)
        {
            return (value & ~(1 << n) | (bit << n));
        }
        private void ChangeBit(int pixelIndex, int bitIndex, int bitSign)
        {
            var mask = 1 << bitIndex;
            if (bitSign == 1)
                pixelData[pixelIndex] |= (byte)mask;
            else
                pixelData[pixelIndex] &= (byte)~mask;
        }
    }
    public class AudioDecoder : AudioSteganography
    {
        public AudioDecoder(WAV carrierToDecode)
        {
            carrier = carrierToDecode;
            randomSeed = carrier.samples;
            CheckCarrier();
            FirstSample();
        }
        private void CheckCarrier()
        {
            foreach (SampleOrder order in new List<SampleOrder> { SampleOrder.Sequential, SampleOrder.Random })
            {
                sampleOrder = order;
                for (int bits = 1; bits <= carrier.bitsPerSample; bits++)
                {
                    bitsToUse = bits;
                    FirstSample();

                    if (UnicodeEncoding.UTF8.GetString(Decode(11)) == "!#encoded#!")
                        return;
                }
            }
            throw new Exception("Carrier not encoded");
        }
        public byte[] Decode(int bytesToDecode)
        {
            List<byte> decodedBytes = new List<byte>();
            for (int i = 0; i < bytesToDecode; i++)
                decodedBytes.Add(DecodeByte());

            return decodedBytes.ToArray();
        }
        private byte DecodeByte()
        {
            uint decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                uint sample = carrier.GetSample2(sampleIndex);
                while (bitsLeft > 0)
                {
                    //int bitSign = pixelData[currentPixel + (int)channelIndex] / ((int)Math.Pow(2, channelBitsLeft[(int)channelIndex] - 1)) % 2;
                    int bitSign = (int)(sample / ((int)Math.Pow(2, bitsLeft - 1)) % 2);
                    decodedByte += (uint)((int)Math.Pow(2, 7 - decodedBits) * bitSign);

                    bitsLeft--;
                    decodedBits++;

                    if (decodedBits == 8)
                        return Convert.ToByte(ReverseBits(decodedByte));
                }

                NextSample();
            }
            return Convert.ToByte(ReverseBits(decodedByte));
        }
        private uint ReverseBits(uint value)
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
