using System;
using Xunit;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public class ResponseTest {
        [Theory]
        [InlineData("SHIORI/3.0 204 No Content\x000d\x000aCharset: UTF-8\x000d\x000aSecurityLevel: Local\xd\xa\xd\xa", 204, "SHIORI", "3.0", "Charset", "UTF-8", "SecurityLevel", "Local")]
        [InlineData("SHIORI/3.0 200 OK\x000d\x000aCharset: UTF-8\x000d\x000aValue: foo\xd\xa\xd\xa", 200, "SHIORI", "3.0", "Charset", "UTF-8", "Value", "foo")]
        [InlineData("SHIORI/2.6 500 Internal Server Error\x000d\x000aSender: ‚³‚­‚ç\x000d\x000aCharset: Shift_JIS\xd\xa\xd\xa", 500, "SHIORI", "2.6", "Sender", "‚³‚­‚ç", "Charset", "Shift_JIS")]
        [InlineData("SAORI/1.0 200 OK\x000d\x000aValue0: foo\xd\xa\xd\xa", 200, "SAORI", "1.0", "Value0", "foo")]
        public void ToStringTest(string result, int statusCode, string protocol, string version, params string[] headers) {
            var instance = new Response{ StatusCode = statusCode, Protocol = protocol, Version = version, Headers = TestCommon.ValuesToHeaders(headers) };
            Assert.Equal(instance.ToString(), result);
            Assert.Equal($"{instance}", result);
        }

        [Fact]
        public void Constructor() {
            Assert.True(new Response(200).StatusCode == 200);
            Assert.True(new Response(400).StatusCode == 400);
        }
    }
}

