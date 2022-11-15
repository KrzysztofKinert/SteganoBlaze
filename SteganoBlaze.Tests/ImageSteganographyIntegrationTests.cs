using Force.Crc32;
using SteganoBlaze.Enums;
using File = System.IO.File;

namespace SteganoBlaze.Tests
{
    public class ImageSteganographyIntegrationTestsGenerator : TheoryData<PixelParameters>
    {
        public ImageSteganographyIntegrationTestsGenerator()
        {
            for (var r = 0; r <= 8; r++)
            for (var g = 0; g <= 8; g++)
            for (var b = 0; b <= 8; b++)
            {
                if (r == 0 && g == 0 && b == 0) continue;
                Add(new PixelParameters(PixelOrder.Sequential, new PixelBits(r, g, b)));
                Add(new PixelParameters(PixelOrder.Random, new PixelBits(r, g, b)));
            }
        }
    }

    public class ImageSteganographyIntegrationTests
    {
        private readonly byte[] _fileToEncode;
        private readonly Image _image;

        public ImageSteganographyIntegrationTests()
        {
            _fileToEncode = File.ReadAllBytes("./Resources/8_bit_sample.wav");
            var pixelData = File.ReadAllBytes("./Resources/PNGSample_PixelData.dat");
            _image = new Image(new Models.File())
            {
                PixelData = pixelData,
                Width = (int)Math.Sqrt(pixelData.Length),
                Height = (int)Math.Sqrt(pixelData.Length)
            };
        }

        //[Theory]
        [Theory(Skip = "Long running")]
        [ClassData(typeof(ImageSteganographyIntegrationTestsGenerator))]
        public void ImageSteganography_ShouldDecodeEncodedFileWithoutDataCorruption(PixelParameters pixelParameters)
        {
            //Arrange
            var encoder = new ImageEncoder(_image, pixelParameters);
            var expected = Crc32Algorithm.Compute(_fileToEncode);

            //Act
            encoder.Encode(_fileToEncode);
            var encodedImage = new Image(new Models.File())
            {
                PixelData = encoder.GetEncodedCarrier(),
                Width = _image.Width,
                Height = _image.Height
            };
            var decoder = new ImageDecoder(encodedImage, pixelParameters);
            var actual = Crc32Algorithm.Compute(decoder.Decode(_fileToEncode.Length));

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}