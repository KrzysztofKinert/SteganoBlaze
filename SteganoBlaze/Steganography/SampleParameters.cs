using SteganoBlaze.Enums;

namespace SteganoBlaze.Steganography
{
	public readonly struct SampleParameters
    {
        public readonly SampleOrder SampleOrder;
        public readonly int SampleBitsToUse;
        public SampleParameters(SampleOrder order, int bits)
        {
            SampleOrder = order;
            SampleBitsToUse = bits;
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
