using Microsoft.AspNetCore.Components.Forms;
using SteganoBlaze.Interfaces;
using SteganoBlaze.Utils;

namespace SteganoBlaze.Models
{
    public class File : IFile
    {
        public byte[] ByteData { get; set; } = Array.Empty<byte>();
        public string Base64Data { get; set; } = "";
        public string ContentType { get; set; } = "";
        public string FileName { get; set; } = "";
        public int FileSize { get; set; }

        public File()
        {

        }
        public File(byte[] fileBytes, IBrowserFile fileInfo)
        {
            ByteData = fileBytes;
            Base64Data = Convert.ToBase64String(fileBytes);

            var cT = fileInfo.ContentType;
            if (cT == "") cT = "application/octet-stream";
            ContentType = cT;

            FileName = fileInfo.Name;
            FileSize = (int)fileInfo.Size;
        }
        public string SizeToString()
        {
            return ByteSize.Reduce(FileSize);
        }
    }
}
