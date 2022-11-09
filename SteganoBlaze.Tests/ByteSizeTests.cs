using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteganoBlaze.Utils;

namespace SteganoBlaze.Tests
{
    public class ByteSizeTests
    {
        [Fact]
        public void Reduce_IntNegativeBytesShouldReturnEmptyString()
        {
            //Arrange
            string expected = "";

            //Act
            string actual = ByteSize.Reduce(-512);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_IntBytesShouldReturnByteString()
        {
            //Arrange
            string expected = "512.00 B";

            //Act
            string actual = ByteSize.Reduce(512);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_IntThousandBytesShouldReturnKiloByteString()
        {
            //Arrange
            string expected = "512.00 KB";

            //Act
            string actual = ByteSize.Reduce(512 * 1024);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_IntMilionBytesShouldReturnMegaByteString()
        {
            //Arrange
            string expected = "512.00 MB";

            //Act
            string actual = ByteSize.Reduce(512 * 1024 * 1024);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_DoubleNegativeBytesShouldReturnEmptyString()
        {
            //Arrange
            string expected = "";

            //Act
            string actual = ByteSize.Reduce(-512.0);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_DoubleBytesShouldReturnByteString()
        {
            //Arrange
            string expected = "512.00 B";

            //Act
            string actual = ByteSize.Reduce(512.0);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_DoubleThousandBytesShouldReturnKiloByteString()
        {
            //Arrange
            string expected = "512.00 KB";

            //Act
            string actual = ByteSize.Reduce(512.0 * 1024);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Reduce_DoubleMilionBytesShouldReturnMegaByteString()
        {
            //Arrange
            string expected = "512.00 MB";

            //Act
            string actual = ByteSize.Reduce(512.0 * 1024 * 1024);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
