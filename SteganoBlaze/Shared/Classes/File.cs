using Microsoft.AspNetCore.Components.Forms;
namespace SteganoBlaze.Shared.Classes
{
    public enum FileType
    {
        Image,
        EncodedImage,
        Audio,
        Message
    }
    public interface IFile
    {
        public byte[] byteData { get; set; }
        public string base64Data { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public int fileSize { get; set; }
    }

    public class File : IFile
    {
        public byte[] byteData { get; set; } = Array.Empty<byte>();
        public string base64Data { get; set; } = "";
        public string contentType { get; set; } = "";
        public string fileName { get; set; } = "";
        public int fileSize { get; set; }

        public File()
        {

        }
        public File(byte[] fileBytes, IBrowserFile fileInfo)
        {
            byteData = fileBytes;
            base64Data = Convert.ToBase64String(fileBytes);

            var cT = fileInfo.ContentType;
            if (cT == "") cT = "application/octet-stream";
            contentType = cT;

            fileName = fileInfo.Name;
            fileSize = (int)fileInfo.Size;
        }
        public string SizeToString()
        {
            return ReduceSize.ToString(fileSize);
        }
    }
}
