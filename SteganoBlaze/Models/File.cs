using Microsoft.AspNetCore.Components.Forms;
using SteganoBlaze.Shared.Classes;
using SteganoBlaze.Interfaces;

namespace SteganoBlaze.Models
{
    public class File : IFile
    {
        public byte[] ByteData { get; set; } = Array.Empty<byte>();
        public string base64Data { get; set; } = "";
        public string ContentType { get; set; } = "";
        public string FileName { get; set; } = "";
        public int FileSize { get; set; }

        public File()
        {

        }
        public File(byte[] fileBytes, IBrowserFile fileInfo)
        {
            ByteData = fileBytes;
            base64Data = Convert.ToBase64String(fileBytes);

            var cT = fileInfo.ContentType;
            if (cT == "") cT = "application/octet-stream";
            ContentType = cT;

            FileName = fileInfo.Name;
            FileSize = (int)fileInfo.Size;
        }
        public string SizeToString()
        {
            return ReduceSize.ToString(FileSize);
        }
    }
}
