using System;
using Xunit;
using ShioriSharp;
using ShioriSharp.Message;

namespace ShioriSharpTest.Container {
    public class StatusLineTest {
        [Theory]
        [InlineData(200, "SHIORI", "2.0")]
        [InlineData(204, "SHIORI", "2.2")]
        [InlineData(310, "SHIORI", "2.3")]
        [InlineData(311, "SHIORI", "2.4")]
        [InlineData(312, "SHIORI", "2.5")]
        [InlineData(400, "SHIORI", "2.6")]
        [InlineData(500, "SHIORI", "3.0")]
        [InlineData(200, "SAORI", "1.0")]
        public void Valid(int statusCode, string protocol, string version) {
            var instance = new StatusLine { StatusCode = statusCode, Protocol = protocol, Version = version };
            Assert.True(instance.Valid);
            Assert.True(instance.Validate() != null);
        }

        [Theory]
        [InlineData(200, "SHIORI", "1.0")]
        [InlineData(900, "SHIORI", "2.6")]
        [InlineData(204, "SAORI", "2.0")]
        public void Invalid(int statusCode, string protocol, string version) {
            var instance = new StatusLine { StatusCode = statusCode, Protocol = protocol, Version = version };
            Assert.False(instance.Valid);
            Assert.Throws<InvalidOperationException>(() => instance.Validate());
        }

        [Theory]
        [InlineData(200, "SHIORI", "3.0", "SHIORI/3.0 200 OK")]
        [InlineData(204, "SHIORI", "3.0", "SHIORI/3.0 204 No Content")]
        [InlineData(310, "SHIORI", "3.0", "SHIORI/3.0 310 Communicate")]
        [InlineData(311, "SHIORI", "3.0", "SHIORI/3.0 311 Not Enough")]
        [InlineData(312, "SHIORI", "3.0", "SHIORI/3.0 312 Advice")]
        [InlineData(400, "SHIORI", "3.0", "SHIORI/3.0 400 Bad Request")]
        [InlineData(500, "SAORI", "1.0", "SAORI/1.0 500 Internal Server Error")]
        public void ToStringTest(int statusCode, string protocol, string version, string result) {
            var instance = new StatusLine { StatusCode = statusCode, Protocol = protocol, Version = version };
            Assert.Equal(instance.ToString(), result);
            Assert.Equal($"{instance}", result);
        }
    }
}

