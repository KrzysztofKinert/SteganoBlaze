using SteganoBlaze.Enums;
using SteganoBlaze.Models;

namespace SteganoBlaze.Steganography
{
    public abstract class ImageSteganography
    {
        protected byte[]? PixelData;

        protected PixelParameters Parameters;
        protected int PixelIndex;
        protected int[] ChannelBitsLeft = new int[3];
        protected Channel Channel;

        protected int RandomSeed;
        protected Random? Generator;
        protected HashSet<int>? UsedPixels;

        protected void FirstPixel()
        {
            if (PixelData is null)
                throw new Exception();

            ResetChannels();

            switch (Parameters.PixelOrder)
            {
                case PixelOrder.Sequential:
                    PixelIndex = 0;
                    break;
                case PixelOrder.Random:
                    Generator = new Random(RandomSeed);
                    UsedPixels = new HashSet<int>();
                    PixelIndex = Generator.Next(PixelData.Length / 4) * 4;
                    UsedPixels.Add(PixelIndex);
                    break;
            }
        }
        protected void NextPixel()
        {
            if (PixelData is null)
                throw new Exception();

            ResetChannels();

            switch (Parameters.PixelOrder)
            {
                case PixelOrder.Sequential:
                    PixelIndex += 4;
                    break;
                case PixelOrder.Random:
                    if (UsedPixels is null || Generator is null)
                        throw new Exception();
                    while (UsedPixels.Contains(PixelIndex))
                        PixelIndex = Generator.Next(PixelData.Length / 4) * 4;
                    UsedPixels.Add(PixelIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void ResetChannels()
        {
            Channel = Channel.R;
            ChannelBitsLeft[(int)Channel.R] = Parameters.PixelBitsToUse.R;
            ChannelBitsLeft[(int)Channel.G] = Parameters.PixelBitsToUse.G;
            ChannelBitsLeft[(int)Channel.B] = Parameters.PixelBitsToUse.B;
        }

        protected int BitIndex() =>
            ChannelBitsLeft[(int)Channel] - 1;
        protected int ChannelIndex() =>
            PixelIndex + (int)Channel;
    }
    public class ImageEncoder : ImageSteganography
    {
        public ImageEncoder(Image carrier, PixelParameters pixelParams)
        {
            PixelData = carrier.PixelData ?? throw new Exception();
            RandomSeed = carrier.Height * carrier.Width;
            Parameters = pixelParams;
            FirstPixel();
        }
        public void Encode(byte[] bytesToEncode)
        {
            foreach (byte byteToEncode in bytesToEncode)
                EncodeByte(byteToEncode);
        }
        public byte[] GetEncodedCarrier() =>
            PixelData ?? throw new Exception();

        private void EncodeByte(byte byteValue)
        {
            if (PixelData is null)
                throw new Exception();

            int encodedBits = 0;
            while (encodedBits < 8)
            {
                if (ChannelBitsLeft[(int)Channel] > 0)
                {
                    var A = PixelData[ChannelIndex()];
                    var B = BitIndex();


                    PixelData[ChannelIndex()] = byteValue % 2 == 1 ? SetBit(ref PixelData[ChannelIndex()], BitIndex())
                                                                   : ClearBit(ref PixelData[ChannelIndex()], BitIndex());

                    byteValue /= 2;
                    ChannelBitsLeft[(int)Channel]--;
                    encodedBits++;
                }
                else
                {
                    if (Channel == Channel.B)
                        NextPixel();
                    else
                        Channel++;
                }
            }
        }
        private static byte SetBit(ref byte value, int bitIndex) =>
            value |= (byte)(1 << bitIndex);
        private static byte ClearBit(ref byte value, int bitIndex) =>
            value &= (byte)~(1 << bitIndex);
    }
    public class ImageDecoder : ImageSteganography
    {
        public ImageDecoder(Image carrierToDecode, PixelParameters pixelParams)
        {
            PixelData = carrierToDecode.PixelData ?? throw new Exception();
            RandomSeed = carrierToDecode.Height * carrierToDecode.Width;
            Parameters = pixelParams;
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
            if (PixelData is null)
                throw new Exception();

            int decodedByte = 0;
            int decodedBits = 0;
            while (decodedBits < 8)
            {
                if (ChannelBitsLeft[(int)Channel] > 0)
                {
                    decodedByte += BitSign(PixelData[ChannelIndex()], BitIndex()) << decodedBits;

                    ChannelBitsLeft[(int)Channel]--;
                    decodedBits++;
                }
                else
                {
                    if (Channel == Channel.B)
                        NextPixel();
                    else
                        Channel++;
                }
            }
            return Convert.ToByte(decodedByte);
        }
        private static int BitSign(uint value, int bitIndex) =>
            (value & 1 << bitIndex) != 0 ? 1 : 0;
    }
}