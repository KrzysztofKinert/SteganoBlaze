using SteganoBlaze.Enums;
using SteganoBlaze.Models;
using SteganoBlaze.Steganography;

namespace SteganoBlaze.Tests
{
    public class AudioEncoderTestsGenerator : TheoryData<SampleParameters>
    {
        public AudioEncoderTestsGenerator()
        {
            for (var bits = 1; bits < 9; bits++)
            {
                Add(new SampleParameters(SampleOrder.Sequential, bits));
                Add(new SampleParameters(SampleOrder.Random, bits));
            }
        }
    }
    public class AudioEncoderTests : AudioSteganography
    {

        [Theory]
        [ClassData(typeof(AudioEncoderTestsGenerator))]
        public void Encode_ShouldEncodeBytesOnSelectedBits(SampleParameters testSampleParameters)
        {
            //Arrange
            var fileBytes = System.IO.File.ReadAllBytes("./Resources/8_bit_sample_2.wav");
            var file = new Models.File() { ByteData = fileBytes };
            Carrier = new Wav(file);

            for (var i = Carrier.FindChunkIndex("data"); i < Carrier.ByteData.Length; i++)
                Carrier.ByteData[i] = 255;

            var encoder = new AudioEncoder(Carrier, testSampleParameters);

            var bytesToEncode = new byte[Carrier.GetTotalSampleCount() * testSampleParameters.SampleBitsToUse / 8];

            var expected = new byte[Carrier.GetTotalSampleCount() * Carrier.BitsPerSample / 8];
            for (var i = 0; i < expected.Length - Carrier.GetTotalSampleCount() * testSampleParameters.SampleBitsToUse % 8; i++)
            {
                expected[i] = (byte)(255 - (Math.Pow(2, testSampleParameters.SampleBitsToUse) - 1));
            }

            //Act
            encoder.Encode(bytesToEncode);
            var actual = encoder.GetEncodedCarrier()[Carrier.FindChunkIndex("data")..];

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
