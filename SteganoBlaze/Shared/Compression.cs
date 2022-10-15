using System.IO.Compression;

namespace SteganoBlaze.Shared
{
    public static class Compression
    {
        public async static Task<byte[]> Compress(byte[] data)
        {
            using MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.SmallestSize))
                await dstream.WriteAsync(data, 0, data.Length);

            return output.ToArray();
        }
        public async static Task<byte[]> Decompress(byte[] data)
        {
            using MemoryStream input = new MemoryStream(data);
            using MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                await dstream.CopyToAsync(output);

            return output.ToArray();
        }
    }
}
