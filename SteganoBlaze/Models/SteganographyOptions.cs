namespace SteganoBlaze.Models
{
    public class SteganographyOptions
    {
        public bool CompressionEnabled { get; set; } = false;
        public bool EncryptionEnabled { get; set; } = false;
        public bool ScatteredEncodingEnabled { get; set; } = false;
        public string? Password { get; set; }
    }
    public class ImageSteganographyOptions : SteganographyOptions
    {
        public bool WebPFormatEnabled { get; set; } = false;
        public PixelBits BitsToEncode { get; set; } = new PixelBits(1, 1, 1);
    }
    public class AudioSteganographyOptions : SteganographyOptions
    {
        public int BitsToEncode { get; set; } = 1;
    }
}
