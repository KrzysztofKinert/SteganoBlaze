using ByteSizeLib;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Text;

namespace SteganoBlaze.Shared.Classes.Types
{
    public class Message
    {
        public byte[] header { get; set; }
        public byte[] metadata { get; set; }
        public byte[] file { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public long messageSize { get; set; }
        public bool compressed { get; set; }
        public Message(byte[] messageFile, IBrowserFile messageFileInfo)
        {
            file = messageFile;

            var cT = messageFileInfo.ContentType;
            if (cT == "") cT = "application/octet-stream";
            contentType = cT;

            fileName = messageFileInfo.Name;
            compressed = false;

            string metadataString = "!fnS!" + fileName + "!fnE!" + "!ctS!" + contentType + "!ctE!" + "!fsS!" + file.Length + "!fsE!" + "!cpS!" + compressed + "!cpE!";
            metadata = UnicodeEncoding.UTF8.GetBytes(metadataString);

            string headerString = "!#encoded#!" + metadata.Length;
            while ((UnicodeEncoding.UTF8.GetByteCount(headerString) % 15) != 0)
            {
                headerString = headerString.Insert(11, "0");
            }
            header = UnicodeEncoding.UTF8.GetBytes(headerString);

            messageSize = header.Length + metadata.Length + file.Length;
        }

        public void UpdateMetaData()
        {
            string metadataString = "!fnS!" + fileName + "!fnE!" + "!ctS!" + contentType + "!ctE!" + "!fsS!" + file.Length + "!fsE!" + "!cpS!" + compressed + "!cpE!";
            metadata = UnicodeEncoding.UTF8.GetBytes(metadataString);
            messageSize = header.Length + metadata.Length + file.Length;
        }
        public void UpdateHeader(byte[] salt, byte[] iv)
        {
            string headerString = "!#encoded#!" + metadata.Length;
            while ((UnicodeEncoding.UTF8.GetByteCount(headerString) % 15) != 0)
            {
                headerString = headerString.Insert(11, "0");
            }

            header = UnicodeEncoding.UTF8.GetBytes(headerString);
            header = header.Concat(salt).ToArray();
            header = header.Concat(iv).ToArray();
            messageSize = header.Length + metadata.Length + file.Length;
        }
        public string SizeToString()
        {
            return ByteSize.FromBytes(messageSize).ToString();
        }
    }
}
