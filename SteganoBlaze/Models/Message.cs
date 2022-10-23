using System.Text;
using SteganoBlaze.Shared;
namespace SteganoBlaze.Models
{
    public class Message : File
    {
        public byte[] Header { get; set; } = Array.Empty<byte>();
        public bool IsCompressed { get; set; }

        //4 4-byte markers + 2-byte filename size + 2-byte type size + 4-byte file size + 1-byte bool = 25 bytes 
        public static readonly int MIN_HEADER_SIZE = 4 * 4 + 2 * 2 + 4 + 1;
        //25-byte min size + 1020-byte UTF-8 filename + 255-byte type = 1300 bytes
        public static readonly int MAX_HEADER_SIZE = MIN_HEADER_SIZE + 255 * 4 + 255;

        public Message(File file)
        {
            ByteData = file.ByteData;
            ContentType = file.ContentType;
            FileName = file.FileName;
            IsCompressed = false;
            UpdateHeader();
        }

        public byte[] GenerateMetadata()
        {
            List<byte> metadata = new();
            metadata.AddRange(Encoding.UTF8.GetBytes("NAME"));
            metadata.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(FileName)));
            metadata.AddRange(Encoding.UTF8.GetBytes(FileName));

            metadata.AddRange(Encoding.UTF8.GetBytes("TYPE"));
            metadata.AddRange(BitConverter.GetBytes((ushort)ContentType.Length));
            metadata.AddRange(Encoding.UTF8.GetBytes(ContentType));

            metadata.AddRange(Encoding.UTF8.GetBytes("SIZE"));
            metadata.AddRange(BitConverter.GetBytes(ByteData.Length));

            metadata.AddRange(Encoding.UTF8.GetBytes("CMPR"));
            metadata.AddRange(BitConverter.GetBytes(IsCompressed));
            return metadata.ToArray();
        }

        public void UpdateHeader(byte[]? metadata = null, List<byte>? aesParams = null)
        {
            List<byte> headerList = (metadata ?? GenerateMetadata()).ToList();

            headerList.InsertRange(0, BitConverter.GetBytes(headerList.Count));
            headerList.InsertRange(sizeof(int), aesParams ?? new List<byte>());

            Header = headerList.ToArray();
        }

        public int GetMessageSize() =>
            Header.Length + ByteData.Length;

        public new string SizeToString() =>
            ByteSize.Reduce(GetMessageSize());
    }
}
