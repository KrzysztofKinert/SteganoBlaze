using SteganoBlaze.Shared.Enums;

namespace SteganoBlaze.Shared.Classes
{
    public readonly struct PixelParams
    {
        public readonly PixelOrder pixelOrder;
        public readonly PixelBits pixelBitsToUse;
        public PixelParams(PixelOrder order, PixelBits bits)
        {
            pixelOrder = order;
            pixelBitsToUse = bits;
        }
    }
    public abstract class ImageSteganography
    {
        protected enum Channel
        {
            R,
            G,
            B
        }
        
        protected byte[] pixelData = Array.Empty<byte>();
        protected PixelParams parameters;
        protected int pixelIndex;
        protected int[] channelBitsLeft = new int[3];
        protected Channel channel;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedPixels;

        protected void FirstPixel()
        {
            channel = Channel.R;
            channelBitsLeft[(int)Channel.R] = parameters.pixelBitsToUse.R;
            channelBitsLeft[(int)Channel.G] = parameters.pixelBitsToUse.G;
            channelBitsLeft[(int)Channel.B] = parameters.pixelBitsToUse.B;

            switch (parameters.pixelOrder)
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
            channelBitsLeft[(int)Channel.R] = parameters.pixelBitsToUse.R;
            channelBitsLeft[(int)Channel.G] = parameters.pixelBitsToUse.G;
            channelBitsLeft[(int)Channel.B] = parameters.pixelBitsToUse.B;

            switch (parameters.pixelOrder)
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
        public ImageEncoder(Image carrier, PixelParams pixelParams)
        {
            if (carrier.pixelData is null)
                throw new Exception();

            pixelData = carrier.pixelData;
            randomSeed = carrier.height * carrier.width;
            parameters = pixelParams;
            FirstPixel();
        }
        public void Encode(byte[] bytesToEncode)
        {
            foreach (byte byteToEncode in bytesToEncode)
                EncodeByte(byteToEncode);
        }
        public byte[] GetEncodedCarrier()
        {
            return pixelData;
        }
        private void EncodeByte(byte byteValue)
        {
            int encodedBits = 0;
            while (encodedBits < 8)
            {
                while (channelBitsLeft[(int)channel] > 0)
                {
                    pixelData[ChannelIndex()] = byteValue % 2 == 1 ? SetBit(pixelData[ChannelIndex()], BitIndex()) 
                                                                   : ClearBit(pixelData[ChannelIndex()], BitIndex());
                    
                    byteValue /= 2;
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
        private static byte SetBit(byte value, int bitIndex) { return value |= (byte)(1 << bitIndex); }
        private static byte ClearBit(byte value, int bitIndex) { return value &= (byte)~(1 << bitIndex); }
    }
    public class ImageDecoder : ImageSteganography
    {
        public ImageDecoder(Image carrierToDecode, PixelParams pixelParams)
        {
            if (carrierToDecode.pixelData is null)
                throw new Exception();

            pixelData = carrierToDecode.pixelData;
            randomSeed = carrierToDecode.height * carrierToDecode.width;
            parameters = pixelParams;
            FirstPixel();
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
        public static List<PixelParams> GenerateParams()
        {
            List<PixelBits> bitsToCheck = new List<PixelBits>();
            for (int r = 0; r < 9; r++)
            {
                for (int g = 0; g < 9; g++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        if (!(r == 0 && g == 0 && b == 0))
                            bitsToCheck.Add(new PixelBits { R = r, G = g, B = b });
                    }
                }
            }

            List<PixelParams> paramsToCheck = new List<PixelParams>();
            foreach(PixelBits pixelBits in bitsToCheck)
                paramsToCheck.Add(new PixelParams(PixelOrder.Sequential, pixelBits));
            foreach (PixelBits pixelBits in bitsToCheck)
                paramsToCheck.Add(new PixelParams(PixelOrder.Random, pixelBits));
            return paramsToCheck;
        }
        private static int ReverseBits(int value)
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