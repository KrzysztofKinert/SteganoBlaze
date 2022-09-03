using ByteSizeLib;
using Microsoft.AspNetCore.Components.Forms;

namespace SteganoBlaze.Shared.Classes.Types
{
    public class Base64File
    {
        public string base64Data { get; set; } = "";
        public string contentType { get; set; } = "";
        public string fileName { get; set; } = "";
        public long fileSize { get; set; }

        public Base64File()
        {

        }
        public Base64File(byte[] file, IBrowserFile fileInfo)
        {
            base64Data = Convert.ToBase64String(file);
            var cT = fileInfo.ContentType;
            if (cT == "") cT = "application/octet-stream";
            contentType = cT;

            fileName = fileInfo.Name;
            fileSize = fileInfo.Size;
        }
        public string SizeToString()
        {
            return ByteSize.FromBytes(fileSize).ToString();
        }
    }
}
