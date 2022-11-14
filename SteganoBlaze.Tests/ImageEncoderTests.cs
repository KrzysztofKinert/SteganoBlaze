using System.Collections;
using SteganoBlaze.Enums;
using SteganoBlaze.Steganography;
using SteganoBlaze.Models;

namespace SteganoBlaze.Tests
{
    public class ImageEncoderTestsGenerator : TheoryData<PixelBits>
    {
        public ImageEncoderTestsGenerator()
        {
            Add(new PixelBits(1, 1, 1));

        }
    }
    public class ImageEncoderTests
	{
        private const int _pixelCount = 16;

        [Fact]
        public void Encode_ShouldModifyBitsInBytes()
        {
            //Arrange
            var image = new Image(new Models.File()) { PixelData = new byte[_pixelCount * 4] };
            var pixelParameters = new PixelParameters(PixelOrder.Sequential, new PixelBits(8, 4, 0));
            var _encoder = new ImageEncoder(image, pixelParameters);

            for (var i = 0; i < _pixelCount * 4; i++)
                image.PixelData[i] = 255;

            var _bytesToEncode = new byte[24];


            var expected = new byte[_pixelCount * 4];
            for (var i = 0; i < _pixelCount * 4; i++)
            {
                expected[i] = (i % 4) switch
                {
                    0 => 0b00000000,
                    1 => 0b11110000,
                    2 => 0b11111111,
                    3 => 0b11111111,
                    _ => expected[i]
                };
            }

            //Act
            _encoder.Encode(_bytesToEncode);
            var actual = _encoder.GetEncodedCarrier();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [ClassData(typeof(ImageEncoderTestsGenerator))]
        public void Encode_ShouldModifyBitsInBytesA(PixelBits testData)
        {
            //Arrange
            var image = new Image(new Models.File()) { PixelData = new byte[_pixelCount * 4] };
            var pixelParameters = new PixelParameters(PixelOrder.Sequential, testData);
            var _encoder = new ImageEncoder(image, pixelParameters);

            for (var i = 0; i < _pixelCount * 4; i++)
                image.PixelData[i] = 255;

            var _bytesToEncode = new byte[20];

            var expected = new byte[_pixelCount * 4];
            for (var i = 0; i < _pixelCount * 4; i++)
            {
                expected[i] = (i % 4) switch
                {
                    0 => (byte)(255 - Math.Pow(2, testData.R)),
                    1 => (byte)(255 - Math.Pow(2, testData.G)),
                    2 => (byte)(255 - Math.Pow(2, testData.B)),
                    3 => 255,
                    _ => expected[i]
                };
            }

            //Act
            _encoder.Encode(_bytesToEncode);
            var actual = _encoder.GetEncodedCarrier();

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
