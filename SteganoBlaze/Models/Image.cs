namespace SteganoBlaze.Models
{
    public class Image : File
    {
        public byte[]? PixelData;
        public int Height;
        public int Width;

        public Image(File file)
        {
            Base64Data = file.Base64Data;
            ContentType = file.ContentType;
            FileName = file.FileName;
            FileSize = file.FileSize;
        }
    }
}
