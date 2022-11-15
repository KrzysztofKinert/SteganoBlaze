using Force.Crc32;
using SteganoBlaze.Enums;
using File = System.IO.File;

namespace SteganoBlaze.Tests
{
    public class AudioSteganographyIntegrationTestsGenerator : TheoryData<SampleParameters>
    {
        public AudioSteganographyIntegrationTestsGenerator()
        {
            for (var bits = 1; bits <= 32; bits++)
            {
                Add(new SampleParameters(SampleOrder.Sequential, bits));
                Add(new SampleParameters(SampleOrder.Random, bits));
            }
        }
    }

    public class AudioSteganographyIntegrationTests
    {
        private readonly Wav _audio;
        private readonly byte[] _fileToEncode;

        public AudioSteganographyIntegrationTests()
        {
            _fileToEncode = File.ReadAllBytes("./Resources/8_bit_sample.wav");
            _audio = new Wav(new Models.File { ByteData = File.ReadAllBytes("./Resources/32_bit_float.wav") });
        }

        [Theory]
        [ClassData(typeof(AudioSteganographyIntegrationTestsGenerator))]
        public void AudioSteganography_ShouldDecodeEncodedFileWithoutDataCorruption(SampleParameters sampleParameters)
        {
            //Arrange
            var encoder = new AudioEncoder(_audio, sampleParameters);
            var expected = Crc32Algorithm.Compute(_fileToEncode);

            //Act
            encoder.Encode(_fileToEncode);
            var encodedAudio = new Wav(new Models.File { ByteData = encoder.GetEncodedCarrier() });
            var decoder = new AudioDecoder(encodedAudio, sampleParameters);
            var actual = Crc32Algorithm.Compute(decoder.Decode(_fileToEncode.Length));

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}