using SteganoBlaze.Enums;
using SteganoBlaze.Models;
using SteganoBlaze.Steganography;

namespace SteganoBlaze.Tests
{
    public class ImageDecoderTestsGenerator : TheoryData<PixelParameters>
    {
        public ImageDecoderTestsGenerator()
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
    public class ImageDecoderTests
	{
        private readonly int _pixelCount = 16;

        [Theory]
        [ClassData(typeof(ImageDecoderTestsGenerator))]
        public void Decode_ShouldDecodeSelectedNumberOfBytes(PixelParameters testData)
        {
            //Arrange
            var image = new Image(new Models.File()) { PixelData = new byte[_pixelCount * 4], Width = (int)Math.Sqrt(_pixelCount), Height = (int)Math.Sqrt(_pixelCount) };
            var decoder = new ImageDecoder(image, testData);

            for (var i = 0; i < _pixelCount * 4; i++)
                image.PixelData[i] = 255;

            var bytesToDecode = _pixelCount * testData.PixelBitsToUse.GetBitSum() / 8;

            var expected = new byte[_pixelCount * testData.PixelBitsToUse.GetBitSum() / 8];
            for (var i = 0; i < expected.Length; i++)
                expected[i] = 255;

            //Act
            var actual = decoder.Decode(bytesToDecode);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
