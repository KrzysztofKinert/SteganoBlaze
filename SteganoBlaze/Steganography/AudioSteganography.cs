using SteganoBlaze.Enums;
using SteganoBlaze.Models;

namespace SteganoBlaze.Steganography
{
    public abstract class AudioSteganography
    {
        protected Wav? Carrier;

        protected SampleParameters Parameters;
        protected int SampleIndex;
        protected int SampleBitsLeft;

        protected int RandomSeed;
        protected Random? Generator;
        protected HashSet<int>? UsedSamples;

        protected void FirstSample()
        {
            if (Carrier is null) 
                throw new Exception();

            SampleBitsLeft = Parameters.SampleBitsToUse;
            switch (Parameters.SampleOrder)
            {
                case SampleOrder.Sequential:
                    SampleIndex = 0;
                    break;
                case SampleOrder.Random:
                    Generator = new Random(RandomSeed);
                    UsedSamples = new HashSet<int>();
                    SampleIndex = Generator.Next(Carrier.GetTotalSampleCount());
                    UsedSamples.Add(SampleIndex);
                    break;
            }
        }
        protected void NextSample()
        {
            if (Carrier is null) 
                throw new Exception();

            SampleBitsLeft = Parameters.SampleBitsToUse;
            switch (Parameters.SampleOrder)
            {
                case SampleOrder.Sequential:
                    SampleIndex++;
                    break;
                case SampleOrder.Random:
                    if (UsedSamples is null || Generator is null)
                        throw new Exception();
                    while (UsedSamples.Contains(SampleIndex))
                        SampleIndex = Generator.Next(Carrier.GetTotalSampleCount());
                    UsedSamples.Add(SampleIndex);
                    break;
            }
        }
        protected int BitIndex() 
            => SampleBitsLeft - 1;
    }
    public class AudioEncoder : AudioSteganography
    {
        public AudioEncoder(Wav audio, SampleParameters sampleParams)
        {
            Carrier = audio.Clone();
            RandomSeed = Carrier.GetTotalSampleCount();
            Parameters = sampleParams;
            FirstSample();
        }
        public void Encode(byte[] bytesToEncode)
        {
            foreach (byte byteToEncode in bytesToEncode)
                EncodeByte(byteToEncode);
        }
        public byte[] GetEncodedCarrier() =>
            Carrier is null ? throw new Exception() : Carrier.GetData();

        private void EncodeByte(byte byteValue)
        {
            if (Carrier is null)
                throw new Exception();

            int encodedBits = 0;
            while (encodedBits < 8)
            {
                if (SampleBitsLeft > 0)
                {
                    uint sample = Carrier.GetSample(SampleIndex);
                    sample = byteValue % 2 == 1 ? SetBit(sample, BitIndex()) 
                                                : ClearBit(sample, BitIndex());
                    Carrier.SetSample(SampleIndex, sample);

                    byteValue /= 2;
                    SampleBitsLeft--;
                    encodedBits++;
                }
                else
                    NextSample();
            }
        }
        private static uint SetBit(uint value, int bitIndex) =>
            value |= (uint)(1 << bitIndex);
        private static uint ClearBit(uint value, int bitIndex) =>
            value &= (uint)~(1 << bitIndex);
    }
    public class AudioDecoder : AudioSteganography
    {
        public AudioDecoder(Wav carrierToDecode, SampleParameters sampleParams)
        {
            Carrier = carrierToDecode;
            RandomSeed = Carrier.GetTotalSampleCount();
            Parameters = sampleParams;
            FirstSample();
        }
        public byte[] Decode(int bytesToDecode)
        {
            List<byte> decodedBytes = new();
            for (int i = 0; i < bytesToDecode; i++)
                decodedBytes.Add(DecodeByte());

            return decodedBytes.ToArray();
        }
        private byte DecodeByte()
        {
            if (Carrier is null) 
                throw new Exception();

            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                uint sample = Carrier.GetSample(SampleIndex);
                if (SampleBitsLeft > 0)
                {
                    decodedByte += BitSign(sample, BitIndex()) << decodedBits;

                    SampleBitsLeft--;
                    decodedBits++;
                }
                else
                    NextSample();
            }
            return Convert.ToByte(decodedByte);
        }
        private static int BitSign(uint value, int bitIndex) =>
            (value & 1 << bitIndex) != 0 ? 1 : 0;

    }
}
