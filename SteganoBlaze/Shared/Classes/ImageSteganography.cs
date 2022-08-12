using SteganoBlaze.Shared.Classes.Types;
using System.Text;
using System.Threading;

namespace SteganoBlaze.Shared.Classes
{
    public abstract class ImageSteganography
    {
        protected byte[] carrier = Array.Empty<byte>();
        protected int currentPixel;
        protected int[] channelBitsLeft = new int[3];
        protected PixelBits bitsToUse = new PixelBits();
        protected enum PixelChannel
        {
            R,
            G,
            B
        }
        protected PixelChannel currentChannel;

        protected enum PixelOrder
        {
            Sequential,
            Random
        }
        protected PixelOrder pixelOrder;
        protected void FirstPixelSequential()
        {
            currentPixel = 0;
            currentChannel = PixelChannel.R;
            channelBitsLeft[(int)PixelChannel.R] = bitsToUse!.R;
            channelBitsLeft[(int)PixelChannel.G] = bitsToUse!.G;
            channelBitsLeft[(int)PixelChannel.B] = bitsToUse!.B;
        }
        protected void NextPixelSequential()
        {
            currentPixel += 4;
            currentChannel = PixelChannel.R;
            channelBitsLeft[(int)PixelChannel.R] = bitsToUse!.R;
            channelBitsLeft[(int)PixelChannel.G] = bitsToUse!.G;
            channelBitsLeft[(int)PixelChannel.B] = bitsToUse!.B;
        }
        protected void NextPixelRandom()
        {

        }
    }
    public class ImageEncoder : ImageSteganography
    {
        public ImageEncoder(byte[] carrierToEncode, PixelBits bits)
        {
            carrier = carrierToEncode;
            bitsToUse = bits;
        }

        public byte[] Encode(Message message)
        {
            FirstPixelSequential();

            for (int i = 0; i < message.header.Length; i++)
            {
                EncodeByte(message.header[i]);
            }
            for (int i = 0; i < message.metadata.Length; i++)
            {
                EncodeByte(message.metadata[i]);
            }
            for (int i = 0; i < message.message.Length; i++)
            {
                EncodeByte(message.message[i]);
            }
            return carrier;
        }
        private void EncodeByte(byte byteValue)
        {
            int encodedBits = 0;
            while (encodedBits < 8)
            {
                while (channelBitsLeft[(int)currentChannel] > 0)
                {
                    int bit = byteValue % 2;
                    byteValue /= 2;
                    //currentPixelValues[(int)currentChannel] = (byte)ChangeNthBit(currentPixelValues[(int)currentChannel], bit, channelBitsLeft[(int)currentChannel] - 1);
                    carrier[currentPixel + (int)currentChannel] = (byte)ChangeNthBit(carrier[currentPixel + (int)currentChannel], bit, channelBitsLeft[(int)currentChannel] - 1);

                    channelBitsLeft[(int)currentChannel]--;
                    encodedBits++;
                    if (encodedBits == 8) return;
                }

                if (currentChannel == PixelChannel.B)
                {
                    NextPixelSequential();
                }
                else currentChannel++;
            }
        }
        private int ChangeNthBit(int value, int bit, int n)
        {
            return (value & ~(1 << n) | (bit << n));
        }

    }
    public class ImageDecoder : ImageSteganography
    {
        public ImageDecoder(byte[] carrierToDecode)
        {
            carrier = carrierToDecode;
            bitsToUse = CheckCarrier(carrier);
        }

        public ByteFile Decode(byte[]? password = null)
        {
            FirstPixelSequential();

            List<byte> decodedBytes = new List<byte>();
            for (int i = 0; i < 16; i++)
            {
                decodedBytes.Add(DecodeByte());
            }
            if (password != null)
            {
                //string header = UnicodeEncoding.Default.GetString(decodedBytes.ToArray());
            }
            string header = UnicodeEncoding.Default.GetString(decodedBytes.ToArray());
            var metadataSize = Int32.Parse(header.TakeLast(5).ToArray());

            decodedBytes = new List<byte>();
            for (int i = 0; i < metadataSize; i++)
            {
                decodedBytes.Add(DecodeByte());
            }

            string metadata = UnicodeEncoding.Default.GetString(decodedBytes.ToArray());

            string fileName = metadata.Substring(metadata.IndexOf("!fnS!") + 5, metadata.IndexOf("!fnE!") - metadata.IndexOf("!fnS!") - 5);
            string contentType = metadata.Substring(metadata.IndexOf("!ctS!") + 5, metadata.IndexOf("!ctE!") - metadata.IndexOf("!ctS!") - 5);
            int fileSize = Int32.Parse(metadata.Substring(metadata.IndexOf("!fsS!") + 5, metadata.IndexOf("!fsE!") - metadata.IndexOf("!fsS!") - 5));

            decodedBytes = new List<byte>();
            for (int i = 0; i < fileSize; i++)
            {
                decodedBytes.Add(DecodeByte());
            }

            return new ByteFile { byteData = decodedBytes.ToArray(), contentType = contentType, fileName = fileName, fileSize = fileSize };
        }

        private PixelBits CheckCarrier(byte[] carrier)
        {
            List<PixelBits> bitsToCheck = new List<PixelBits>();
            for (int i = 1; i < 9; i++)
            {
                bitsToCheck.Add(new PixelBits { R = i, G = i, B = i });
            }
            for (int r = 0; r < 9; r++)
            {
                for (int g = 0; g < 9; g++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        if (!(r == g && g == b))
                        {
                            {
                                bitsToCheck.Add(new PixelBits { R = r, G = g, B = b });
                            }
                        }
                    }
                }
            }

            foreach (PixelBits bits in bitsToCheck)
            {
                bitsToUse = bits;
                FirstPixelSequential();

                List<byte> decodedBytes = new List<byte>();
                for (int i = 0; i < 11; i++)
                {
                    decodedBytes.Add(DecodeByte());
                }
                string decodedString = UnicodeEncoding.Default.GetString(decodedBytes.ToArray());
                if (decodedString == "!#encoded#!")
                {
                    return bits;
                }
            }

            throw new Exception("Carrier not encoded");
            //return null;
        }
        private byte DecodeByte()
        {
            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                while (channelBitsLeft[(int)currentChannel] > 0)
                {
                    int bitSign = carrier[currentPixel + (int)currentChannel] / ((int)Math.Pow(2, channelBitsLeft[(int)currentChannel] - 1)) % 2;
                    decodedByte += (int)Math.Pow(2, 7 - decodedBits) * bitSign;

                    channelBitsLeft[(int)currentChannel]--;
                    decodedBits++;
                    if (decodedBits == 8) return Convert.ToByte(ReverseBits(decodedByte));
                }

                if (currentChannel == PixelChannel.B)
                {
                    NextPixelSequential();
                }
                else currentChannel++;
            }
            return Convert.ToByte(ReverseBits(decodedByte));
        }
        private int ReverseBits(int value)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + value % 2;
                value /= 2;
            }

            return result;
        }
    }
}

