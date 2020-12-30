using System;
using Xunit;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public class RequestTest {
        [Theory]
        [InlineData("GET SHIORI/3.0\x000d\x000aCharset: UTF-8\x000d\x000aSecurityLevel: Local\xd\xa\xd\xa", "GET", "SHIORI", "3.0", "Charset", "UTF-8", "SecurityLevel", "Local")]
        [InlineData("NOTIFY SHIORI/3.0\x000d\x000aCharset: UTF-8\x000d\x000aValue: foo\xd\xa\xd\xa", "NOTIFY", "SHIORI", "3.0", "Charset", "UTF-8", "Value", "foo")]
        [InlineData("GET Version SHIORI/2.6\x000d\x000aSender: embryo\x000d\x000aCharset: Shift_JIS\xd\xa\xd\xa", "GET Version", "SHIORI", "2.6", "Sender", "embryo", "Charset", "Shift_JIS")]
        [InlineData("EXECUTE SAORI/1.0\x000d\x000aArgument0: foo\xd\xa\xd\xa", "EXECUTE", "SAORI", "1.0", "Argument0", "foo")]
        public void ToStringTest(string result, string method, string protocol, string version, params string[] headers) {
            var instance = new Request{ Method = method, Protocol = protocol, Version = version, Headers = TestCommon.ValuesToHeaders(headers) };
            Assert.Equal(instance.ToString(), result);
            Assert.Equal($"{instance}", result);
        }

        [Fact]
        public void Constructor() {
            Assert.True(new Request("GET").Method == "GET");
            Assert.True(new Request("NOTIFY").Method == "NOTIFY");
        }
    }
}

