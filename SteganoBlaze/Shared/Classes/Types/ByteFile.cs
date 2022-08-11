using ByteSizeLib;

namespace SteganoBlaze.Shared.Classes.Types
{
    public class ByteFile
    {
        public byte[] byteData { get; set; } = Array.Empty<byte>();
        public string contentType { get; set; } = "";
        public string fileName { get; set; } = "";
        public long fileSize { get; set; }

        public string SizeToString()
        {
            return ByteSize.FromBytes(fileSize).ToString();
        }
    }
}
