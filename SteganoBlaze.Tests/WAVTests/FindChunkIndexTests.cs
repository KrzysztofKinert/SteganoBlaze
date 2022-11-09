using SteganoBlaze.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SteganoBlaze.Models;

namespace SteganoBlaze.Tests.WAVTests
{
    public class FindChunkIndexTests
    {
        [Fact]
        public void FindChunkIndex_ShouldFindFMTChunkIn24_bit_fixed()
        {
            //Arrange
            var fileName = "./Resources/24_bit_fixed.WAV";
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var expected = 6120;

            //Act
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);
            var actual = wav.FindChunkIndex("fmt ");

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindChunkIndex_ShouldFindFMTChunkIn32_bit_float()
        {
            //Arrange
            var fileName = "./Resources/32_bit_float.WAV";
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var expected = 6120;

            //Act
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);
            var actual = wav.FindChunkIndex("fmt ");

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindChunkIndex_ShouldFindFMTChunkSampleWAV()
        {
            //Arrange
            var fileName = "./Resources/SampleWAV.WAV";
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var expected = 20;

            //Act
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);
            var actual = wav.FindChunkIndex("fmt ");

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindChunkIndex_ShouldFindDataChunkIn24_bit_fixed()
        {
            //Arrange
            var fileName = "./Resources/24_bit_fixed.WAV";
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var expected = 6144;

            //Act
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);
            var actual = wav.FindChunkIndex("data");

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindChunkIndex_ShouldFindDataChunkIn32_bit_float()
        {
            //Arrange
            var fileName = "./Resources/32_bit_float.WAV";
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var expected = 6144;

            //Act
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);
            var actual = wav.FindChunkIndex("data");

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindChunkIndex_ShouldFindDataChunkSampleWAV()
        {
            //Arrange
            var fileName = "./Resources/SampleWAV.WAV";
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            var expected = 44;

            //Act
            var file = new Models.File() { ByteData = fileBytes };
            var wav = new WAV(file);
            var actual = wav.FindChunkIndex("data");

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
