using SteganoBlaze.Enums;
using SteganoBlaze.Models;

namespace SteganoBlaze.Steganography
{
	public readonly struct PixelParameters
    {
        public readonly PixelOrder pixelOrder;
        public readonly PixelBits pixelBitsToUse;
        public PixelParameters(PixelOrder order, PixelBits bits)
        {
            pixelOrder = order;
            pixelBitsToUse = bits;
        }
        public static List<PixelParameters> GenerateParameters()
        {
            List<PixelBits> bitsToCheck = new();
            for (int i = 1; i < 9; i++)
                bitsToCheck.Add(new PixelBits(i, i, i));

            for (int r = 0; r < 9; r++)
            {
                for (int g = 0; g < 9; g++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        if (!(r == g && g == b))
                            bitsToCheck.Add(new PixelBits(r, g, b));
                    }
                }
            }

            List<PixelParameters> paramsToCheck = new();
            foreach (PixelBits pixelBits in bitsToCheck)
                paramsToCheck.Add(new PixelParameters(PixelOrder.Sequential, pixelBits));
            foreach (PixelBits pixelBits in bitsToCheck)
                paramsToCheck.Add(new PixelParameters(PixelOrder.Random, pixelBits));
            return paramsToCheck;
        }
    }
}