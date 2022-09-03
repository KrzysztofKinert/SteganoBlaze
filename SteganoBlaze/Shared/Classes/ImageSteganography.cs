using Microsoft.JSInterop;
using SteganoBlaze.Shared.Classes.Types;
using System.Text;
using System.Threading;
namespace SteganoBlaze.Shared.Classes
{
    public abstract class ImageSteganography
    {
        protected enum Channel
        {
            R,
            G,
            B
        }
        public enum PixelOrder
        {
            Sequential,
            Random
        }

        protected byte[] pixelData = Array.Empty<byte>();
        protected int pixelIndex;
        protected int[] channelBitsLeft = new int[3];
        protected PixelBits bitsToUse = new PixelBits();

        protected Channel channel;
        protected PixelOrder pixelOrder;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedPixels;

        protected void FirstPixel()
        {
            channel = Channel.R;
            channelBitsLeft[(int)Channel.R] = bitsToUse!.R;
            channelBitsLeft[(int)Channel.G] = bitsToUse!.G;
            channelBitsLeft[(int)Channel.B] = bitsToUse!.B;

            switch (pixelOrder)
            {
                case PixelOrder.Sequential:
                    pixelIndex = 0;
                    break;
                case PixelOrder.Random:
                    generator = new Random(randomSeed);
                    usedPixels = new HashSet<int>();
                    pixelIndex = generator.Next(pixelData.Length / 4) * 4;
                    usedPixels.Add(pixelIndex);
                    break;
            }
        }
        protected void NextPixel()
        {
            channel = Channel.R;
            channelBitsLeft[(int)Channel.R] = bitsToUse!.R;
            channelBitsLeft[(int)Channel.G] = bitsToUse!.G;
            channelBitsLeft[(int)Channel.B] = bitsToUse!.B;

            switch (pixelOrder)
            {
                case PixelOrder.Sequential:
                    pixelIndex += 4;
                    break;
                case PixelOrder.Random:
                    while (usedPixels!.Contains(pixelIndex))
                        pixelIndex = generator!.Next(pixelData.Length / 4) * 4;
                    usedPixels.Add(pixelIndex);
                    break;
            }
        }
        protected int BitIndex() { return channelBitsLeft[(int)channel] - 1; }
        protected int ChannelIndex() { return pixelIndex + (int)channel; }
    }
    public class ImageEncoder : ImageSteganography
    {
        public ImageEncoder(Image carrier, PixelBits bits, bool randomEncodingEnabled)
        {
            pixelData = carrier.pixelData;
            bitsToUse = bits;
            randomSeed = carrier.height * carrier.width;

            if (randomEncodingEnabled)
                pixelOrder = PixelOrder.Random;
            else
                pixelOrder = PixelOrder.Sequential;

            FirstPixel();
        }
        public byte[] Encode(Message message)
        {
            foreach(byte headerByte in message.header)
                EncodeByte(headerByte);

            foreach (byte metadataByte in message.metadata)
                EncodeByte(metadataByte);

            foreach (byte fileByte in message.file)
                EncodeByte(fileByte);

            return pixelData;
        }
        private void EncodeByte(byte byteValue)
        {
            int encodedBits = 0;
            while (encodedBits < 8)
            {
                while (channelBitsLeft[(int)channel] > 0)
                {
                    int bitSign = byteValue % 2;
                    byteValue /= 2;
                    ChangeBit(ChannelIndex(), BitIndex(), bitSign);

                    channelBitsLeft[(int)channel]--;
                    encodedBits++;

                    if (encodedBits == 8) 
                        return;
                }

                switch (channel)
                {
                    case Channel.R:
                        channel = Channel.G;
                        break;
                    case Channel.G:
                        channel = Channel.B;
                        break;
                    case Channel.B:
                        NextPixel();
                        break;  
                }
            }
        }
        private void ChangeBit(int pixelIndex, int bitIndex, int bitSign)
        {
            var mask = 1 << bitIndex;
            if (bitSign == 1)
                pixelData[pixelIndex] |= (byte)mask;
            else
                pixelData[pixelIndex] &= (byte)~mask;
        }
    }
    public class ImageDecoder : ImageSteganography, IDecoder
    {
        public ImageDecoder(Image carrierToDecode)
        {
            pixelData = carrierToDecode.pixelData;
            randomSeed = carrierToDecode.height * carrierToDecode.width;
            CheckCarrier();
            FirstPixel();
        }
        private void CheckCarrier()
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

            foreach (PixelOrder order in new List<PixelOrder> { PixelOrder.Sequential, PixelOrder.Random })
            {
                pixelOrder = order;
                foreach (PixelBits bits in bitsToCheck)
                {
                    bitsToUse = bits;
                    FirstPixel();

                    if (UnicodeEncoding.UTF8.GetString(Decode(11)) == "!#encoded#!")
                        return;
                }
            }
            throw new Exception("Carrier not encoded");
        }
        public byte[] Decode(int bytesToDecode)
        {
            List<byte> decodedBytes = new List<byte>();
            for (int i = 0; i < bytesToDecode; i++)
                decodedBytes.Add(DecodeByte());

            return decodedBytes.ToArray();
        }
        private byte DecodeByte()
        {
            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                while (channelBitsLeft[(int)channel] > 0)
                {
                    //int bitSign = pixelData[currentPixel + (int)channelIndex] / ((int)Math.Pow(2, channelBitsLeft[(int)channelIndex] - 1)) % 2;
                    int bitSign = pixelData[ChannelIndex()] / ((int)Math.Pow(2, BitIndex())) % 2;
                    decodedByte += (int)Math.Pow(2, 7 - decodedBits) * bitSign;

                    channelBitsLeft[(int)channel]--;
                    decodedBits++;

                    if (decodedBits == 8) 
                        return Convert.ToByte(ReverseBits(decodedByte));
                }

                if (channel == Channel.B)
                    NextPixel();
                else 
                    channel++;
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