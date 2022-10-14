namespace SteganoBlaze.Shared.Interfaces
{
    public interface IFile
    {
        public byte[] byteData { get; set; }
        public string base64Data { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public int fileSize { get; set; }
    }
}
