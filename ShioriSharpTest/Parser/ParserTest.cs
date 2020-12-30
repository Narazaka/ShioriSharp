using Xunit;
using ShioriSharp.Message;
using ShioriSharp.Parser;

namespace ShioriSharpTest.Parser {
    public class ProtocolTest {
        const string CRLF = "\xd\xa";

        public static object[][] RequestData() => new object[][] {
            new object[] {
                $"GET SHIORI/3.0{CRLF}Charset: UTF-8{CRLF}Sender: さくら{CRLF}{CRLF}",
                new Request { Headers = { Charset = "UTF-8", Sender = "さくら" } },
            },
            new object[] {
                $"NOTIFY SHIORI/3.0{CRLF}Foo:Bar:  : : : {CRLF}Foo Bar: \x1\x2{CRLF}{CRLF}",
                new Request("NOTIFY") { Headers = { { "Foo:Bar", " : : : " }, { "Foo Bar", "\x1\x2" } } },
            },
            new object[] {
                $"GET Version SAORI/1.0{CRLF}Charset: UTF-8{CRLF}Sender: さくら{CRLF}{CRLF}",
                new Request("GET Version", "SAORI") { Headers = { Charset = "UTF-8", Sender = "さくら" } },
            },
        };

        [Theory]
        [MemberData(nameof(RequestData))]
        public void Request(string input, Request result) {
            Assert.Equal(ShioriSharp.Parser.Parser.ParseRequest(input).ToString(), result.ToString());
        }

        public static object[][] ResponseData() => new object[][] {
            new object[] {
                $"SHIORI/3.0 204 No Content{CRLF}Charset: UTF-8{CRLF}Sender: さくら{CRLF}{CRLF}",
                new Response(204) { Headers = { Charset = "UTF-8", Sender = "さくら" } },
            },
            new object[] {
                $"SHIORI/3.0 500 Internal Server Error{CRLF}Foo:Bar:  : : : {CRLF}Foo Bar: \x1\x2{CRLF}{CRLF}",
                new Response(500) { Headers = { { "Foo:Bar", " : : : " }, { "Foo Bar", "\x1\x2" } } },
            },
            new object[] {
                $"SAORI/1.0 200 OK{CRLF}Charset: UTF-8{CRLF}Sender: さくら{CRLF}{CRLF}",
                new Response(200, "SAORI") { Headers = { Charset = "UTF-8", Sender = "さくら" } },
            },
        };

        [Theory]
        [MemberData(nameof(ResponseData))]
        public void Response(string input, Response result) {
            Assert.Equal(ShioriSharp.Parser.Parser.ParseResponse(input).ToString(), result.ToString());
        }
    }
}

