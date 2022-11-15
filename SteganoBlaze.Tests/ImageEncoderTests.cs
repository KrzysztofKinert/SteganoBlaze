using SteganoBlaze.Enums;
using File = SteganoBlaze.Models.File;

namespace SteganoBlaze.Tests
{
    public class ImageEncoderTestsGenerator : TheoryData<PixelParameters>
    {
        public ImageEncoderTestsGenerator()
        {
            for (var r = 0; r < 9; r++)
            for (var g = 0; g < 9; g++)
            for (var b = 0; b < 9; b++)
            {
                Add(new PixelParameters(PixelOrder.Sequential, new PixelBits(r, g, b)));
                Add(new PixelParameters(PixelOrder.Random, new PixelBits(r, g, b)));
            }
        }
    }

    public class ImageEncoderTests
    {
        private readonly int _pixelCount = 16;

        [Theory]
        [ClassData(typeof(ImageEncoderTestsGenerator))]
        public void Encode_ShouldEncodeBytesOnSelectedBits(PixelParameters testPixelParameters)
        {
            //Arrange
            var image = new Image(new File())
            {
                PixelData = new byte[_pixelCount * 4], Width = (int)Math.Sqrt(_pixelCount),
                Height = (int)Math.Sqrt(_pixelCount)
            };
            var encoder = new ImageEncoder(image, testPixelParameters);

            for (var i = 0; i < _pixelCount * 4; i++)
                image.PixelData[i] = 255;

            var bytesToEncode = new byte[_pixelCount * testPixelParameters.PixelBitsToUse.GetBitSum() / 8];

            var expected = new byte[_pixelCount * 4];
            for (var i = 0; i < _pixelCount * 4; i++)
                expected[i] = (i % 4) switch
                {
                    0 => (byte)(255 - (Math.Pow(2, testPixelParameters.PixelBitsToUse.R) - 1)),
                    1 => (byte)(255 - (Math.Pow(2, testPixelParameters.PixelBitsToUse.G) - 1)),
                    2 => (byte)(255 - (Math.Pow(2, testPixelParameters.PixelBitsToUse.B) - 1)),
                    3 => 255,
                    _ => expected[i]
                };

            //Act
            encoder.Encode(bytesToEncode);
            var actual = encoder.GetEncodedCarrier();

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}