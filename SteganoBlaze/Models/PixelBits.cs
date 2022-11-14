namespace SteganoBlaze.Models
{
    public class PixelBits
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }

        public PixelBits(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public int GetBitSum() =>
            R + G + B;
    }
}
