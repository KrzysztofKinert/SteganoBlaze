using ByteSizeLib;
using System.Text;
namespace SteganoBlaze.Shared.Classes
{
    public class Message
    {
        public byte[] header { get; set; } = Array.Empty<byte>();
        public byte[] fileData { get; set; } = Array.Empty<byte>();
        public string contentType { get; set; }
        public string fileName { get; set; }
        public bool compressed { get; set; }

        //4 4-byte markers + 2-byte filename size + 2-byte type size + 4-byte file size + 1-byte bool = 25 bytes 
        public static readonly int MIN_HEADER_SIZE = 4 * 4 + 2 * 2 + 4 + 1;
        //25-byte min size + 1020-byte UTF-8 filename + 255-byte type = 1300 bytes
        public static readonly int MAX_HEADER_SIZE = MIN_HEADER_SIZE + 255 * 4 + 255;

        public Message(File file)
        {
            fileData = file.byteData;
            contentType = file.contentType;
            fileName = file.fileName;
            compressed = false;
            UpdateHeader();
        }

        public byte[] GenerateMetadata()
        {
            List<byte> metadata = new List<byte>();
            metadata.AddRange(Encoding.UTF8.GetBytes("NAME"));
            metadata.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(fileName)));
            metadata.AddRange(Encoding.UTF8.GetBytes(fileName));

            metadata.AddRange(Encoding.UTF8.GetBytes("TYPE"));
            metadata.AddRange(BitConverter.GetBytes((ushort)contentType.Length));
            metadata.AddRange(Encoding.UTF8.GetBytes(contentType));

            metadata.AddRange(Encoding.UTF8.GetBytes("SIZE"));
            metadata.AddRange(BitConverter.GetBytes(fileData.Length));

            metadata.AddRange(Encoding.UTF8.GetBytes("CMPR"));
            metadata.AddRange(BitConverter.GetBytes(compressed));
            return metadata.ToArray();
        }
        public void UpdateHeader(byte[]? metadata = null, List<byte>? aesParams = null)
        {
            List<byte> headerList = (metadata ?? GenerateMetadata()).ToList();

            headerList.InsertRange(0, BitConverter.GetBytes((ushort)headerList.Count));
            headerList.InsertRange(2, aesParams ?? new List<byte>());

            header = headerList.ToArray();
        }
        public int GetMessageSize() { return header.Length + fileData.Length; }
        public string SizeToString() { return ByteSize.FromBytes(GetMessageSize()).ToString(); }
        public static File ParseMetadata(byte[] header, ref bool isCompressed)
        {
            File file = new File();

            int offset = 0;
            if (Encoding.UTF8.GetString(header, offset, 4) != "NAME")
                throw new Exception();
            int fileNameSize = BitConverter.ToInt16(header, offset + 4);
            file.fileName = Encoding.UTF8.GetString(header, offset + 6, fileNameSize);

            offset += fileNameSize + 6;
            var lol = Encoding.UTF8.GetString(header, offset, 4);
            if (Encoding.UTF8.GetString(header, offset, 4) != "TYPE")
                throw new Exception();
            int contentTypeSize = BitConverter.ToInt16(header, offset + 4);
            file.contentType = Encoding.UTF8.GetString(header, offset + 6, contentTypeSize);

            offset += contentTypeSize + 6;
            if (Encoding.UTF8.GetString(header, offset, 4) != "SIZE")
                throw new Exception();
            file.fileSize = BitConverter.ToInt32(header, offset + 4);

            offset += 8;
            if (Encoding.UTF8.GetString(header, offset, 4) != "CMPR")
                throw new Exception();
            isCompressed = BitConverter.ToBoolean(header, offset + 4);

            return file;
        }
    }
}
