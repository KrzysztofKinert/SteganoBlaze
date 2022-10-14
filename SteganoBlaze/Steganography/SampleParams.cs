using SteganoBlaze.Enums;

namespace SteganoBlaze.Steganography
{
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
}
