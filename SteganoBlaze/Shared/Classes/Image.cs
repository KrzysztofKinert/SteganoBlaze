namespace SteganoBlaze.Shared.Classes
{
    public class Image : File
    {
        public byte[]? pixelData;
        public int height;
        public int width;

        public Image(File file)
        {
            base64Data = file.base64Data;
            contentType = file.contentType;
            fileName = file.fileName;
            fileSize = file.fileSize;
        }
    }
}
