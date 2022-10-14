namespace SteganoBlaze.Interfaces
{
    public interface IFile
    {
        public byte[] ByteData { get; set; }
        public string base64Data { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
    }
}
