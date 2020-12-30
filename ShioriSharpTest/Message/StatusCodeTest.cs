using Xunit;
using ShioriSharp;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public class StatusCodeTest {
        [Theory]
        [InlineData("OK")]
        [InlineData("No Content")]
        [InlineData("Communicate")]
        [InlineData("Not Enough")]
        [InlineData("Advice")]
        [InlineData("Bad Request")]
        [InlineData("Internal Server Error")]
        public void ValidString(string value) {
            Assert.True(((StatusCode)value).Valid);
        }

        [Theory]
        [InlineData(200)]
        [InlineData(204)]
        [InlineData(310)]
        [InlineData(311)]
        [InlineData(312)]
        [InlineData(400)]
        [InlineData(500)]
        public void ValidInt(int value) {
            Assert.True(((StatusCode)value).Valid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2.1")]
        [InlineData("Foo")]
        [InlineData(null)]
        public void InvalidString(string value) {
            Assert.False(((StatusCode)value).Valid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        public void InvalidInt(int value) {
            Assert.False(((StatusCode)value).Valid);
        }
    }
}

