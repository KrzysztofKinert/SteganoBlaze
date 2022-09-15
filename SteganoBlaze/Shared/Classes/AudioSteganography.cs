namespace SteganoBlaze.Shared.Classes
{
    public enum SampleOrder
    {
        Sequential,
        Random
    }
    public readonly struct SampleParams
    {
        public readonly SampleOrder sampleOrder;
        public readonly int sampleBitsToUse;
        public SampleParams(SampleOrder order, int bits)
        {
            sampleOrder = order;
            sampleBitsToUse = bits;
        }
    }
    public abstract class AudioSteganography
	{
        protected WAV? carrier;

        protected SampleParams parameters;
        protected int sampleIndex;
        protected int sampleBitsLeft;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedSamples;

        protected void FirstSample()
        {
            if (carrier is null) throw new Exception();

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
            if (carrier is null) throw new Exception();

            sampleBitsLeft = parameters.sampleBitsToUse;
            switch (parameters.sampleOrder)
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
        public AudioEncoder(WAV audio, SampleParams sampleParams)
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
        public byte[] GetEncodedCarrier()
        {
            return carrier is null ? throw new Exception() : carrier.GetData();
        }

        private void EncodeByte(byte byteValue)
        {
            if (carrier is null) 
                throw new Exception();

            int encodedBits = 0;
            while (encodedBits < 8)
            {
                uint sample = carrier.GetSample(sampleIndex);
                while (sampleBitsLeft > 0)
                {
                    sample = (byteValue % 2 == 1) ? SetBit(sample, BitIndex()) : ClearBit(sample, BitIndex());
                    byteValue /= 2;

                    sampleBitsLeft--;
                    encodedBits++;
                    
                    if (encodedBits == 8)
                    {
                        carrier.SetSample(sampleIndex, sample);
                        return;
                    }
                }
                carrier.SetSample(sampleIndex, sample);
                NextSample();
            }
        }
        private static uint SetBit(uint value, int bitIndex) { return value |= (uint)(1 << bitIndex); }
        private static uint ClearBit(uint value, int bitIndex) { return value &= (uint)~(1 << bitIndex); }
    }
    public class AudioDecoder : AudioSteganography
    {
        public AudioDecoder(WAV carrierToDecode, SampleParams sampleParams)
        {
            carrier = carrierToDecode;
            randomSeed = carrier.GetSampleCount();
            parameters = sampleParams;
            FirstSample();
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
        public static List<SampleParams> GenerateParams(int maxSampleBits)
        {
            List<SampleParams> paramsToCheck = new List<SampleParams>();
            for (int bits = 1; bits <= maxSampleBits; bits++)
                paramsToCheck.Add(new SampleParams(SampleOrder.Sequential, bits));
            for (int bits = 1; bits <= maxSampleBits; bits++)
                paramsToCheck.Add(new SampleParams(SampleOrder.Random, bits));
            return paramsToCheck;
        }
        private static int BitSign(uint value, int bitIndex) { return ((value & (1 << bitIndex)) != 0) ? 1 : 0; }
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
