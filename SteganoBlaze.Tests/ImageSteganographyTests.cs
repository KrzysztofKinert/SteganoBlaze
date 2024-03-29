﻿using SteganoBlaze.Enums;

namespace SteganoBlaze.Tests
{
    public class ImageSteganographyTests : ImageSteganography
    {
        public ImageSteganographyTests()
        {
            var width = 2;
            var height = 2;
            PixelData = new byte[width * height * 4];
            RandomSeed = width * height;
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(8, 8, 8)]
        public void ResetChannels_ShouldResetChannelToRAndChannelBitsLeftToParameters(int r, int g, int b)
        {
            //Arrange
            Parameters = new PixelParameters(PixelOrder.Sequential, new PixelBits(r, g, b));
            var expectedChannelBitsLeft = new int[3];

            var expectedChannel = Channel.R;
            expectedChannelBitsLeft[(int)Channel.R] = r;
            expectedChannelBitsLeft[(int)Channel.G] = g;
            expectedChannelBitsLeft[(int)Channel.B] = b;

            //Act
            ResetChannels();
            var actualChannel = Channel;
            var actualChannelBitsLeft = ChannelBitsLeft;

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
            if (PixelData is null)
                throw new Exception();
            Parameters = new PixelParameters(order, new PixelBits(1, 1, 1));

            var expected = order switch
            {
                PixelOrder.Sequential => 0,
                PixelOrder.Random => 12,
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };

            //Act
            FirstPixel();
            var actual = PixelIndex;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(PixelOrder.Sequential)]
        [InlineData(PixelOrder.Random)]
        public void NextPixel_ShouldSetPixelToNextInOrder(PixelOrder order)
        {
            //Arrange
            if (PixelData is null)
                throw new Exception();
            Parameters = new PixelParameters(order, new PixelBits(1, 1, 1));

            var expected = order switch
            {
                PixelOrder.Sequential => 4,
                PixelOrder.Random => 8,
                _ => 0
            };

            //Act
            FirstPixel();
            NextPixel();
            var actual = PixelIndex;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NextPixel_ShouldNotRepeatPixelsInRandomOrder()
        {
            //Arrange
            if (PixelData is null)
                throw new Exception();
            Parameters = new PixelParameters(PixelOrder.Random, new PixelBits(1, 1, 1));
            var expected = new HashSet<int>();

            for (var i = 0; i < PixelData.Length / 4; i++)
                expected.Add(i * 4);

            //Act
            FirstPixel();
            for (var i = 0; i < (PixelData.Length - 1) / 4; i++)
                NextPixel();
            var actual = UsedPixels;

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}