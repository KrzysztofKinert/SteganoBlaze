using SteganoBlaze.Enums;
using File = System.IO.File;

namespace SteganoBlaze.Tests
{
    public class AudioDecoderTestsGenerator : TheoryData<SampleParameters>
    {
        public AudioDecoderTestsGenerator()
        {
            for (var bits = 1; bits < 9; bits++)
            {
                Add(new SampleParameters(SampleOrder.Sequential, bits));
                Add(new SampleParameters(SampleOrder.Random, bits));
            }
        }
    }

    public class AudioDecoderTests : AudioSteganography
    {
        [Theory]
        [ClassData(typeof(AudioDecoderTestsGenerator))]
        public void Decode_ShouldDecodeSelectedNumberOfBytes(SampleParameters testData)
        {
            //Arrange
            var fileBytes = File.ReadAllBytes("./Resources/8_bit_sample_2.wav");
            var file = new Models.File { ByteData = fileBytes };
            Carrier = new Wav(file);
            var decoder = new AudioDecoder(Carrier, testData);

            for (var i = Carrier.FindChunkIndex("data"); i < Carrier.ByteData.Length; i++)
                Carrier.ByteData[i] = 255;

            var bytesToDecode = Carrier.GetTotalSampleCount() * testData.SampleBitsToUse / 8;

            var expected = new byte[Carrier.GetTotalSampleCount() * testData.SampleBitsToUse / 8];
            for (var i = 0; i < expected.Length; i++)
                expected[i] = 255;

            //Act
            var actual = decoder.Decode(bytesToDecode);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}