namespace SteganoBlaze.Shared.Classes.Types
{
    public class PixelBits
    {
        public int R { get; set; } = 1;
        public int G { get; set; } = 1;
        public int B { get; set; } = 1;

        public int GetBitSum()
        {
            return R + G + B;
        }
    }
}
