using Xunit;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public class VersionTest {
        [Theory]
        [InlineData("1.0")]
        [InlineData("2.0")]
        [InlineData("2.2")]
        [InlineData("2.3")]
        [InlineData("2.4")]
        [InlineData("2.5")]
        [InlineData("2.6")]
        [InlineData("3.0")]
        public void ValidString(string value) {
            Assert.True(((Version)value).Valid);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(2.0)]
        [InlineData(2.2)]
        [InlineData(2.3)]
        [InlineData(2.4)]
        [InlineData(2.5)]
        [InlineData(2.6)]
        [InlineData(3.0)]
        public void ValidDouble(double value) {
            Assert.True(((Version)value).Valid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2.1")]
        [InlineData("Foo")]
        [InlineData(null)]
        public void InvalidString(string value) {
            Assert.False(((Version)value).Valid);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(9.9)]
        public void InvalidDouble(double value) {
            Assert.False(((Version)value).Valid);
        }
    }
}

