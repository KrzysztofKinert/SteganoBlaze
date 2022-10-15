using SteganoBlaze.Enums;

namespace SteganoBlaze.Steganography
{
	public readonly struct SampleParameters
    {
        public readonly SampleOrder sampleOrder;
        public readonly int sampleBitsToUse;
        public SampleParameters(SampleOrder order, int bits)
        {
            sampleOrder = order;
            sampleBitsToUse = bits;
        }
        public static List<SampleParameters> GenerateParameters(int maxSampleBits)
        {
            List<SampleParameters> paramsToCheck = new();
            for (int bits = 1; bits <= maxSampleBits; bits++)
                paramsToCheck.Add(new SampleParameters(SampleOrder.Sequential, bits));
            for (int bits = 1; bits <= maxSampleBits; bits++)
                paramsToCheck.Add(new SampleParameters(SampleOrder.Random, bits));
            return paramsToCheck;
        }
    }
}
