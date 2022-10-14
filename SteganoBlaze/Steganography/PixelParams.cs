using SteganoBlaze.Enums;
using SteganoBlaze.Models;

namespace SteganoBlaze.Steganography
{
	public readonly struct PixelParams
    {
        public readonly PixelOrder pixelOrder;
        public readonly PixelBits pixelBitsToUse;
        public PixelParams(PixelOrder order, PixelBits bits)
        {
            pixelOrder = order;
            pixelBitsToUse = bits;
        }
    }
}