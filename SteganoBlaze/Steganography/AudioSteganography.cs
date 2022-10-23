using SteganoBlaze.Enums;
using SteganoBlaze.Models;

namespace SteganoBlaze.Steganography
{
    public abstract class AudioSteganography
    {
        protected WAV? carrier;

        protected SampleParameters parameters;
        protected int sampleIndex;
        protected int sampleBitsLeft;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedSamples;

        protected void FirstSample()
        {
            if (carrier is null) 
                throw new Exception();

            sampleBitsLeft = parameters.sampleBitsToUse;
            switch (parameters.sampleOrder)
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
            if (carrier is null) 
                throw new Exception();

            sampleBitsLeft = parameters.sampleBitsToUse;
            switch (parameters.sampleOrder)
            {
                case SampleOrder.Sequential:
                    sampleIndex++;
                    break;
                case SampleOrder.Random:
                    if (usedSamples is null || generator is null)
                        throw new Exception();
                    while (usedSamples.Contains(sampleIndex))
                        sampleIndex = generator.Next(carrier.GetSampleCount());
                    usedSamples.Add(sampleIndex);
                    break;
            }
        }
        protected int BitIndex() 
            => sampleBitsLeft - 1;
    }
    public class AudioEncoder : AudioSteganography
    {
        public AudioEncoder(WAV audio, SampleParameters sampleParams)
        {
            carrier = audio.Clone();
            randomSeed = carrier.GetSampleCount();
            parameters = sampleParams;
            FirstSample();
        }
        public void Encode(byte[] bytesToEncode)
        {
            foreach (byte byteToEncode in bytesToEncode)
                EncodeByte(byteToEncode);
        }
        public byte[] GetEncodedCarrier() =>
            carrier is null ? throw new Exception() : carrier.GetData();

        private void EncodeByte(byte byteValue)
        {
            if (carrier is null)
                throw new Exception();

            int encodedBits = 0;
            while (encodedBits < 8)
            {
                if (sampleBitsLeft > 0)
                {
                    uint sample = carrier.GetSample(sampleIndex);
                    sample = byteValue % 2 == 1 ? SetBit(sample, BitIndex()) 
                                                : ClearBit(sample, BitIndex());
                    carrier.SetSample(sampleIndex, sample);

                    byteValue /= 2;
                    sampleBitsLeft--;
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
        public AudioDecoder(WAV carrierToDecode, SampleParameters sampleParams)
        {
            carrier = carrierToDecode;
            randomSeed = carrier.GetSampleCount();
            parameters = sampleParams;
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
            if (carrier is null) 
                throw new Exception();

            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                uint sample = carrier.GetSample(sampleIndex);
                if (sampleBitsLeft > 0)
                {
                    decodedByte += BitSign(sample, BitIndex()) << decodedBits;

                    sampleBitsLeft--;
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
