using SteganoBlaze.Enums;
using SteganoBlaze.Models;
using SteganoBlaze.Steganography;

namespace SteganoBlaze.Tests
{
    public class ImageSteganographyTests : ImageSteganography
    {
        public ImageSteganographyTests()
        {
            var width = 2;
            var height = 2;
            pixelData = new byte[width * height * 4];
            randomSeed = width * height;
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(8, 8, 8)]
        public void ResetChannels_ShouldResetChannelToRAndChannelBitsLeftToParameters(int R, int G, int B)
        {
            //Arrange
            parameters = new PixelParameters(PixelOrder.Sequential, new PixelBits(R, G, B));
            int[] expectedChannelBitsLeft = new int[3];

            var expectedChannel = Channel.R;
            expectedChannelBitsLeft[(int)Channel.R] = R;
            expectedChannelBitsLeft[(int)Channel.G] = G;
            expectedChannelBitsLeft[(int)Channel.B] = B;

            //Act
            ResetChannels();
            var actualChannel = channel;
            var actualChannelBitsLeft = channelBitsLeft;

            //Assert
            Assert.Equal(expectedChannel, actualChannel);
            Assert.Equal(expectedChannelBitsLeft, actualChannelBitsLeft);    
        }

        [Theory]
        [InlineData(PixelOrder.Sequential)]
        [InlineData(PixelOrder.Random)]
        public void FirstPixel_ShouldSetPixelToFirstInOrder(PixelOrder order)
        {
            //Arrange
            if (pixelData is null)
                throw new Exception();
            parameters = new PixelParameters(order, new PixelBits(1, 1, 1));
            int expected = 0;

            switch (order)
            {
                case PixelOrder.Sequential:
                    expected = 0;
                    break;
                case PixelOrder.Random:
                    var generator = new Random(randomSeed);
                    expected = generator.Next(pixelData.Length / 4) * 4;
                    break;
                default:
                    break;
            }

            //Act
            FirstPixel();
            var actual = pixelIndex;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(PixelOrder.Sequential)]
        [InlineData(PixelOrder.Random)]
        public void NextPixel_ShouldSetPixelToNextInOrder(PixelOrder order)
        {
            //Arrange
            if (pixelData is null)
                throw new Exception();
            parameters = new PixelParameters(order, new PixelBits(1, 1, 1));

            var expected = 0;
            switch (order)
            {
                case PixelOrder.Sequential:
                    expected = 4;
                    break;
                case PixelOrder.Random:
                    expected = 8;
                    break;
                default:
                    break;
            }

            //Act
            FirstPixel();
            NextPixel();
            var actual = pixelIndex;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NextPixel_ShouldNotRepeatPixelsInRandomOrder()
        {
            //Arrange
            if (pixelData is null)
                throw new Exception();
            parameters = new PixelParameters(PixelOrder.Random, new PixelBits(1, 1, 1));
            var expected = new HashSet<int>();

            for (int i = 0; i < pixelData.Length / 4; i++)
                expected.Add(i * 4);

            //Act
            FirstPixel();
            for (int i = 0; i < (pixelData.Length - 1) / 4; i++)
                NextPixel();
            var actual = usedPixels;

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
