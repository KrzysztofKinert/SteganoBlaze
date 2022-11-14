using SteganoBlaze.Enums;
using SteganoBlaze.Models;
using SteganoBlaze.Steganography;

namespace SteganoBlaze.Tests
{
    public class AudioSteganographyTests : AudioSteganography
    {
        public AudioSteganographyTests()
        {
            var fileBytes = System.IO.File.ReadAllBytes("./Resources/SampleWAV.wav");
            var file = new Models.File() { ByteData = fileBytes };
            Carrier = new Wav(file);
            RandomSeed = Carrier.GetTotalSampleCount();
        }

        [Theory]
        [InlineData(SampleOrder.Sequential)]
        [InlineData(SampleOrder.Random)]
        public void FirstSample_ShouldSetSampleToFirstInOrder(SampleOrder order)
        {
            //Arrange
            if (Carrier is null)
                throw new Exception();

            Parameters = new SampleParameters(order, 1);

            var expected = order switch
            {
                SampleOrder.Sequential => 0,
                SampleOrder.Random => 173950,
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };

            //Act
            FirstSample();
            var actual = SampleIndex;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(SampleOrder.Sequential)]
        [InlineData(SampleOrder.Random)]
        public void NextPixel_ShouldSetPixelToNextInOrder(SampleOrder order)
        {
            //Arrange
            if (Carrier is null)
                throw new Exception();

            Parameters = new SampleParameters(order, 1);

            var expected = order switch
            {
                SampleOrder.Sequential => 1,
                SampleOrder.Random => 148484,
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };

            //Act
            FirstSample();
            NextSample();
            var actual = SampleIndex;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NextPixel_ShouldNotRepeatPixelsInRandomOrder()
        {
            //Arrange
            if (Carrier is null)
                throw new Exception();

            Parameters = new SampleParameters(SampleOrder.Random, 1);
            var expected = new HashSet<int>();

            for (int i = 0; i < Carrier.GetTotalSampleCount(); i++)
                expected.Add(i);

            //Act
            FirstSample();
            for (int i = 0; i < Carrier.GetTotalSampleCount() - 1; i++)
                NextSample();
            var actual = UsedSamples;

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
