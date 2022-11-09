using SteganoBlaze.Models;

namespace SteganoBlaze.Tests
{
    public class WAVTests
    {
        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", "fmt ", 6120)]
        [InlineData("./Resources/32_bit_float.WAV", "fmt ", 6120)]
        [InlineData("./Resources/SampleWAV.WAV", "fmt ", 20)]
        [InlineData("./Resources/PinkPanther60.WAV", "fmt ", 20)]
        [InlineData("./Resources/8_bit_sample.WAV", "fmt ", 20)]
        [InlineData("./Resources/24_bit_fixed.WAV", "data", 6144)]
        [InlineData("./Resources/32_bit_float.WAV", "data", 6144)]
        [InlineData("./Resources/SampleWAV.WAV", "data", 44)]
        [InlineData("./Resources/PinkPanther60.WAV", "data", 44)]
        [InlineData("./Resources/8_bit_sample.WAV", "data", 44)]
        public void FindChunkIndex_ShouldFindChunkIndex(string fileName, string chunkName, int expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.FindChunkIndex(chunkName);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", 2)]
        [InlineData("./Resources/32_bit_float.WAV", 2)]
        [InlineData("./Resources/SampleWAV.WAV", 2)]
        [InlineData("./Resources/PinkPanther60.WAV", 1)]
        [InlineData("./Resources/8_bit_sample.WAV", 1)]
        public void ParseWAV_ShouldParseWAVChannels(string fileName, int expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.channels;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", 48000)]
        [InlineData("./Resources/32_bit_float.WAV", 48000)]
        [InlineData("./Resources/SampleWAV.WAV", 8000)]
        [InlineData("./Resources/PinkPanther60.WAV", 22050)]
        [InlineData("./Resources/8_bit_sample.WAV", 8000)]
        public void ParseWAV_ShouldParseWAVSampleRate(string fileName, int expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.sampleRate;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", 24)]
        [InlineData("./Resources/32_bit_float.WAV", 32)]
        [InlineData("./Resources/SampleWAV.WAV", 16)]
        [InlineData("./Resources/PinkPanther60.WAV", 16)]
        [InlineData("./Resources/8_bit_sample.WAV", 8)]
        public void ParseWAV_ShouldParseWAVBitsPerSample(string fileName, int expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.BitsPerSample;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", 324266)]
        [InlineData("./Resources/32_bit_float.WAV", 324194)]
        [InlineData("./Resources/SampleWAV.WAV", 268237)]
        [InlineData("./Resources/PinkPanther60.WAV", 1323000)]
        [InlineData("./Resources/8_bit_sample.WAV", 53499)]
        public void ParseWAV_ShouldParseWAVSampleCount(string fileName, int expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.sampleCount;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", 324266 * 2)]
        [InlineData("./Resources/32_bit_float.WAV", 324194 * 2)]
        [InlineData("./Resources/SampleWAV.WAV", 268237 * 2)]
        [InlineData("./Resources/PinkPanther60.WAV", 1323000)]
        [InlineData("./Resources/8_bit_sample.WAV", 53499)]
        public void GetTotalSampleCount_ShouldReturnTotalSampleCount(string fileName, int expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.GetTotalSampleCount();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("./Resources/24_bit_fixed.WAV", "0:06")]
        [InlineData("./Resources/32_bit_float.WAV", "0:06")]
        [InlineData("./Resources/SampleWAV.WAV", "0:33")]
        [InlineData("./Resources/PinkPanther60.WAV", "1:00")]
        [InlineData("./Resources/8_bit_sample.WAV", "0:06")]
        public void GetDuration_ShouldReturnDuration(string fileName, string expected)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);

            //Act
            var actual = wav.GetDuration();

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
