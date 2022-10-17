using SteganoBlaze.Enums;
using SteganoBlaze.Models;

namespace SteganoBlaze.Steganography
{
    public abstract class ImageSteganography
    {
        protected byte[]? pixelData;

        protected PixelParameters parameters;
        protected int pixelIndex;
        protected int[] channelBitsLeft = new int[3];
        protected Channel channel;

        protected int randomSeed;
        protected Random? generator;
        protected HashSet<int>? usedPixels;

        protected void FirstPixel()
        {
            if (pixelData is null)
                throw new Exception();

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
            if (pixelData is null)
                throw new Exception();

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
        protected int BitIndex() =>
            channelBitsLeft[(int)channel] - 1;
        protected int ChannelIndex() =>
            pixelIndex + (int)channel;
    }
    public class ImageEncoder : ImageSteganography
    {
        public ImageEncoder(Image carrier, PixelParameters pixelParams)
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
        public byte[] GetEncodedCarrier() =>
            pixelData is null ? throw new Exception() : pixelData;

        private void EncodeByte(byte byteValue)
        {
            if (pixelData is null)
                throw new Exception();

            int encodedBits = 0;
            while (encodedBits < 8)
            {
                if (channelBitsLeft[(int)channel] > 0)
                {
                    pixelData[ChannelIndex()] = byteValue % 2 == 1 ? SetBit(pixelData[ChannelIndex()], BitIndex())
                                                                   : ClearBit(pixelData[ChannelIndex()], BitIndex());

                    byteValue /= 2;
                    channelBitsLeft[(int)channel]--;
                    encodedBits++;
                }
                else
                {
                    if (channel == Channel.B)
                        NextPixel();
                    else
                        channel++;
                }
            }
        }
        private static byte SetBit(byte value, int bitIndex) =>
            value |= (byte)(1 << bitIndex);
        private static byte ClearBit(byte value, int bitIndex) =>
            value &= (byte)~(1 << bitIndex);
    }
    public class ImageDecoder : ImageSteganography
    {
        public ImageDecoder(Image carrierToDecode, PixelParameters pixelParams)
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
            List<byte> decodedBytes = new();
            for (int i = 0; i < bytesToDecode; i++)
                decodedBytes.Add(DecodeByte());

            return decodedBytes.ToArray();
        }
        private byte DecodeByte()
        {
            if (pixelData is null)
                throw new Exception();

            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                if (channelBitsLeft[(int)channel] > 0)
                {
                    decodedByte += BitSign(pixelData[ChannelIndex()], BitIndex()) << decodedBits;

                    channelBitsLeft[(int)channel]--;
                    decodedBits++;
                }
                else
                {
                    if (channel == Channel.B)
                        NextPixel();
                    else
                        channel++;
                }
            }
            return Convert.ToByte(decodedByte);
        }
        private static int BitSign(uint value, int bitIndex) =>
            (value & 1 << bitIndex) != 0 ? 1 : 0;
    }
}