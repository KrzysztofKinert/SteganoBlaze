namespace SteganoBlaze.Models
{
    public class Image : File
    {
        public byte[]? pixelData;
        public int height;
        public int width;

        public Image(File file)
        {
            base64Data = file.base64Data;
            ContentType = file.ContentType;
            FileName = file.FileName;
            FileSize = file.FileSize;
        }
    }
}
