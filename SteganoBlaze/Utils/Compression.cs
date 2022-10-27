using System.IO.Compression;

namespace SteganoBlaze.Utils
{
    public static class Compression
    {
        public async static Task<byte[]> Compress(byte[] data)
        {
            using MemoryStream output = new();
            using (DeflateStream dstream = new(output, CompressionLevel.SmallestSize))
                await dstream.WriteAsync(data, 0, data.Length);

            return output.ToArray();
        }
        public async static Task<byte[]> Decompress(byte[] data)
        {
            using MemoryStream input = new(data);
            using MemoryStream output = new();
            using (DeflateStream dstream = new(input, CompressionMode.Decompress))
                await dstream.CopyToAsync(output);

            return output.ToArray();
        }
    }
}
