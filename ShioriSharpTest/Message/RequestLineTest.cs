using System;
using Xunit;
using ShioriSharp;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public class RequestLineTest {
        [Theory]
        [InlineData("GET", "SHIORI", "3.0")]
        [InlineData("NOTIFY", "SHIORI", "3.0")]
        [InlineData("GET Version", "SHIORI", "2.0")]
        [InlineData("NOTIFY OwnerGhostName", "SHIORI", "2.0")]
        [InlineData("GET Sentence", "SHIORI", "2.0")]
        [InlineData("GET Word", "SHIORI", "2.0")]
        [InlineData("GET Status", "SHIORI", "2.0")]
        [InlineData("GET Sentence", "SHIORI", "2.2")]
        [InlineData("GET Sentence", "SHIORI", "2.3")]
        [InlineData("NOTIFY OtherGhostName", "SHIORI", "2.3")]
        [InlineData("TEACH", "SHIORI", "2.4")]
        [InlineData("GET String", "SHIORI", "2.5")]
        [InlineData("GET Sentence", "SHIORI", "2.6")]
        [InlineData("GET Status", "SHIORI", "2.6")]
        [InlineData("GET String", "SHIORI", "2.6")]
        [InlineData("NOTIFY OwnerGhostName", "SHIORI", "2.6")]
        [InlineData("NOTIFY OtherGhostName", "SHIORI", "2.6")]
        [InlineData("GET Version", "SHIORI", "2.6")]
        [InlineData("TRANSLATE Sentence", "SHIORI", "2.6")]
        [InlineData("GET Version", "SAORI", "1.0")]
        [InlineData("EXECUTE", "SAORI", "1.0")]
        public void Valid(string method, string protocol, string version) {
            var instance = new RequestLine { Method = method, Protocol = protocol, Version = version };
            Assert.True(instance.Valid);
            Assert.True(instance.Validate() != null);
        }

        [Theory]
        [InlineData("GET", "SHIORI", "2.6")]
        [InlineData("", "SHIORI", "2.6")]
        [InlineData("GET", "SHIORI", "")]
        public void Invalid(string method, string protocol, string version) {
            var instance = new RequestLine { Method = method, Protocol = protocol, Version = version };
            Assert.False(instance.Valid);
            Assert.Throws<InvalidOperationException>(() => instance.Validate());
        }

        [Theory]
        [InlineData("NOTIFY", "SHIORI", "3.0", "NOTIFY SHIORI/3.0")]
        [InlineData("GET Version", "SHIORI", "2.0", "GET Version SHIORI/2.0")]
        public void ToStringTest(string method, string protocol, string version, string result) {
            var instance = new RequestLine { Method = method, Protocol = protocol, Version = version };
            Assert.Equal(instance.ToString(), result);
            Assert.Equal($"{instance}", result);
        }
    }
}

