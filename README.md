# ShioriSharp

![build](https://github.com/Narazaka/ShioriSharp/workflows/build/badge.svg)
![Nuget](https://img.shields.io/nuget/v/ShioriSharp)

SHIORI/2.x,3.x SAORI/1.0 Protocol Parser / Container

<img src="https://raw.githubusercontent.com/Narazaka/ShioriSharp/master/ShioriSharp.png" width="128" height="128">

## Install

available on [NuGet](https://www.nuget.org/packages/ShioriSharp/)

## Usage

```csharp
using Xunit;
using ShioriSharp.Message;
using ShioriSharp.Parser;

namespace Demo {
    public class Demo {
        const string CRLF = "\xd\xa";

        [Fact]
        public void ParseSHIORI() {
            var requestStr = $"GET SHIORI/3.0{CRLF}Sender: さくら{CRLF}{CRLF}";
            var request = Parser.ParseRequest(requestStr);
            Assert.True(request.Valid);
            Assert.True(request.Method == "GET");
            Assert.True(request.Method == Method.Enum.GET);
            Assert.True(request.Version == 3.0);
            Assert.True(request.Headers.Sender == "さくら");
            Assert.Equal(requestStr, request.ToString());
        }

        [Fact]
        public void BuildSHIORI() {
            var response = new Response(200) { Headers = { Sender = "まゆら", Value = @"\hあー。\e" } };
            Assert.Equal(response.ToString(), $"SHIORI/3.0 200 OK{CRLF}Sender: まゆら{CRLF}Value: \\hあー。\\e{CRLF}{CRLF}");
            Assert.Equal("SHIORI/3.0 200 OK", Parser.ParseResponse(response.ToString()).StatusLine.ToString());
        }

        [Fact]
        public void BuildSAORI() {
            var request = new Request("EXECUTE", "SAORI") { Headers = { { "Argument0", "Foo" } } };
            Assert.Equal("Foo", request.Headers.Get("Argument0"));
        }
    }
}
```

## License

[Zlib License](LICENSE)
