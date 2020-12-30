using Xunit;
using ShioriSharp;
using ShioriSharp.Message;

namespace ShioriSharpTest.Container {
    public class ProtocolTest {
        [Theory]
        [InlineData("SHIORI")]
        [InlineData("SAORI")]
        public void ValidString(string value) {
            Assert.True(((Protocol)value).Valid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("MAYUNA")]
        [InlineData("SERIKO")]
        [InlineData(null)]
        public void InvalidString(string value) {
            Assert.False(((Protocol)value).Valid);
        }
    }
}

