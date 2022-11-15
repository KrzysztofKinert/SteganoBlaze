using SteganoBlaze.Utils;

namespace SteganoBlaze.Tests
{
    public class ByteSizeTests
    {
        [Theory]
        [InlineData(-512, "")]
        [InlineData(512, "512.00 B")]
        [InlineData(512 * 1024, "512.00 KB")]
        [InlineData(512 * 1024 * 1024, "512.00 MB")]
        public void Reduce_IntByteSizeShouldReturnReducedByteSizeString(int bytes, string expected)
        {
            //Arrange

            //Act
            var actual = ByteSize.Reduce(bytes);

            //Assert
            Assert.True(expected == actual || expected == actual.Replace(",", "."));
        }

        [Theory]
        [InlineData(-512.0, "")]
        [InlineData(512.0, "512.00 B")]
        [InlineData(512.0 * 1024, "512.00 KB")]
        [InlineData(512.0 * 1024 * 1024, "512.00 MB")]
        public void Reduce_DoubleByteSizeShouldReturnReducedByteSizeString(double bytes, string expected)
        {
            //Arrange

            //Act
            var actual = ByteSize.Reduce(bytes);

            //Assert
            Assert.True(expected == actual || expected == actual.Replace(",", "."));
        }
    }
}