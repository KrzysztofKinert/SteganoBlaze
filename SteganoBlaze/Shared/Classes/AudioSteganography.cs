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

        protected WAV? carrier;
        protected int sampleIndex;
        protected int sampleBitsLeft;
        protected int bitsToUse;

        protected SampleOrder sampleOrder;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedSamples;

        protected void FirstSample()
        {
            if (carrier is null) throw new Exception();

            sampleBitsLeft = bitsToUse;
            switch (sampleOrder)
            {
                case SampleOrder.Sequential:
                    sampleIndex = 0;
                    break;
                case SampleOrder.Random:
                    generator = new Random(randomSeed);
                    usedSamples = new HashSet<int>();
                    sampleIndex = generator.Next(carrier.GetSampleCount());
                    usedSamples.Add(sampleIndex);
                    break;
            }
        }
        protected void NextSample()
        {
            if (carrier is null) throw new Exception();

            sampleBitsLeft = bitsToUse;
            switch (sampleOrder)
            {
                case SampleOrder.Sequential:
                    sampleIndex++;
                    break;
                case SampleOrder.Random:
                    while (usedSamples!.Contains(sampleIndex))
                        sampleIndex = generator!.Next(carrier!.GetSampleCount());
                    usedSamples.Add(sampleIndex);
                    break;
            }
        }
        protected int BitIndex() { return sampleBitsLeft - 1; }
    }
	public class AudioEncoder : AudioSteganography
	{
        public AudioEncoder(WAV audio, int bits, bool randomEncodingEnabled)
        {
            carrier = new WAV((byte[])audio.GetData().Clone());
            //carrier = audio.Clone();
            bitsToUse = bits;
            randomSeed = carrier.GetSampleCount();

            if (randomEncodingEnabled)
                sampleOrder = SampleOrder.Random;
            else
                sampleOrder = SampleOrder.Sequential;

            FirstSample();
        }
        public byte[] Encode(Message message)
        {
            if (carrier is null) throw new Exception();

            foreach (byte headerByte in message.header)
                EncodeByte(headerByte);

            foreach (byte metadataByte in message.metadata)
                EncodeByte(metadataByte);

            foreach (byte fileByte in message.fileData)
                EncodeByte(fileByte);

            return carrier.GetData();
        }
        private void EncodeByte(byte byteValue)
        {
            if (carrier is null) throw new Exception();

            int encodedBits = 0;
            while (encodedBits < 8)
            {
                while (sampleBitsLeft > 0)
                {
                    uint sample = carrier.GetSample(sampleIndex);

                    if (byteValue % 2 == 1)
                        sample = SetBit(sample, BitIndex());
                    else
                        sample = ClearBit(sample, BitIndex());

                    carrier.SetSample(sampleIndex, sample);
                    var aa = carrier.GetSample(sampleIndex);
                    byteValue /= 2;
                    sampleBitsLeft--;
                    encodedBits++;

                    if (encodedBits == 8)
                        return;
                }
                NextSample();
            }
        }
        private static uint SetBit(uint value, int bitIndex)
        {
            value |= (uint)(1 << bitIndex);
            return value;
        }
        private static uint ClearBit(uint value, int bitIndex)
        {
            value &= (uint)~(1 << bitIndex);
            return value;
        }
    }
    public class AudioDecoder : AudioSteganography, IDecoder
    {
        public AudioDecoder(WAV carrierToDecode)
        {
            carrier = carrierToDecode;
            randomSeed = carrier.GetSampleCount();
            CheckCarrier();
            FirstSample();
        }
        private void CheckCarrier()
        {
            if (carrier is null) throw new Exception();

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
            if (carrier is null) throw new Exception();

            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                uint sample = carrier.GetSample(sampleIndex);

                if (sampleBitsLeft > 0)
                {
                    decodedByte += BitSign(sample, BitIndex()) << (7 - decodedBits);

                    sampleBitsLeft--;
                    decodedBits++;
                }
                else
                    NextSample();
            }
            return Convert.ToByte(ReverseBits(decodedByte));
        }
        private static int BitSign(uint value, int bitIndex)
        {
            if ((value & (1 << bitIndex)) != 0) return 1;
            else return 0;
        }
        private static int ReverseBits(int value)
        {
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + value % 2;
                value /= 2;
            }
            return result;
        }
    }
}
