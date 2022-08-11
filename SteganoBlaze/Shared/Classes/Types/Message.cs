using ByteSizeLib;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace SteganoBlaze.Shared.Classes.Types
{
    public class Message
    {
        public byte[] header { get; set; }
        public byte[] metadata { get; set; }
        public byte[] message { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public long fileSize { get; set; }

        public Message(byte[] file, IBrowserFile fileInfo)
        {
            message = file;

            var cT = fileInfo.ContentType;
            if (cT == "") cT = "application/octet-stream";
            contentType = cT;

            fileName = fileInfo.Name;
            fileSize = fileInfo.Size;

            string metaData = "!fnS!" + fileInfo.Name + "!fnE!" + "!ctS!" + contentType + "!ctE!" + "!fsS!" + fileInfo.Size + "!fsE!";
            while ((UnicodeEncoding.UTF8.GetByteCount(metaData) % 16) != 0)
            {
                metaData += "0";
            }
            metadata = UnicodeEncoding.UTF8.GetBytes(metaData);

            string header = "!#encoded#!" + metadata.Length;
            while ((UnicodeEncoding.UTF8.GetByteCount(header) % 16) != 0)
            {
                header = header.Insert(11, "0");
            }
            this.header = UnicodeEncoding.UTF8.GetBytes(header);
        }

        public string SizeToString()
        {
            return ByteSize.FromBytes(fileSize).ToString();
        }
    }
}
