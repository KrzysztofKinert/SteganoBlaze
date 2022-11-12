namespace SteganoBlaze.Models
{
    public class PixelBits
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

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
