namespace SteganoBlaze.Shared.Classes.Types
{
	public class Options
	{
        public bool compressionEnabled { get; set; } = false;
        public bool encryptionEnabled { get; set; } = false;
        public bool scatteredEncodingEnabled { get; set; } = false;
        public string? password { get; set; }
    }
	public class ImageOptions : Options
    {
        public bool webPFormatEnabled { get; set; } = false;
        public PixelBits bitsToEncode { get; set; } = new PixelBits();
	}
    public class AudioOptions : Options
    {
        public int bitsToEncode { get; set; } = 1;
    }
}
