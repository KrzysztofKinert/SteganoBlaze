using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteganoBlaze.Enums;
using SteganoBlaze.Models;
using SteganoBlaze.Steganography;

namespace SteganoBlaze.Tests
{
    public class ImageSteganographyIntegrationTestsGenerator : TheoryData<PixelParameters>
    {
        public ImageSteganographyIntegrationTestsGenerator()
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
    public class ImageSteganographyIntegrationTests
    {
        public ImageSteganographyIntegrationTests()
        {
            var fileBytes = System.IO.File.ReadAllBytes("./Resources/SampleWAV.wav");
            var file = new Models.File() { ByteData = fileBytes };
        }

    }
}
