using System;

namespace ShioriSharp.Parser {
    using Message;

    public static class Parser {
        public static Request ParseRequest(string message) {
            var lineBreakIndex = message.IndexOf(Common.LineSeparator);
            return new() {
                RequestLine = ParseRequestLine(message.Substring(0, lineBreakIndex)),
                Headers = ParseHeaders(message.Substring(lineBreakIndex + 2)),
            };
        }

        public static Response ParseResponse(string message) {
            var lineBreakIndex = message.IndexOf(Common.LineSeparator);
            return new() {
                StatusLine = ParseStatusLine(message.Substring(0, lineBreakIndex)),
                Headers = ParseHeaders(message.Substring(lineBreakIndex + 2)),
            };
        }

        public static RequestLine ParseRequestLine(string line) {
            var protocolSeparatorIndex = line.IndexOf('/');
            if (protocolSeparatorIndex == -1)
                throw new FormatException();
            var protocolStr = line.Substring(protocolSeparatorIndex - 6, 6);
            var protocol = protocolStr == "SHIORI" ? Protocol.SHIORI : protocolStr == " SAORI" ? Protocol.SAORI : throw new FormatException();
            Version version = line.Substring(protocolSeparatorIndex + 1);
            if (version.AsEnum == 0)
                throw new FormatException();
            Method method = line.Substring(0, protocolSeparatorIndex - 7);
            if (method.AsEnum == 0)
                throw new FormatException();
            return new() { Method = method, Protocol = protocol, Version = version };
        }

        public static StatusLine ParseStatusLine(string line) {
            var protocolStr = line.Substring(0, 6);
            var protocol = protocolStr == "SHIORI" ? Protocol.SHIORI : protocolStr == "SAORI/" ? Protocol.SAORI : throw new FormatException();
            var firstSpaceIndex = line.IndexOf(' ');
            if (firstSpaceIndex == -1)
                throw new FormatException();
            var versionStartIndex = protocol.AsEnum == ProtocolEnum.SHIORI ? 7 : 6;
            Version version = line.Substring(versionStartIndex, firstSpaceIndex - versionStartIndex);
            if (version.AsEnum == 0)
                throw new FormatException();
            var secondSpaceIndex = line.IndexOf(' ', firstSpaceIndex + 1);
            if (secondSpaceIndex == -1)
                throw new FormatException();
            StatusCode statusCode = int.Parse(line.Substring(firstSpaceIndex + 1, secondSpaceIndex - firstSpaceIndex - 1));
            if (statusCode.AsEnum == 0)
                throw new FormatException();
            return new() { Protocol = protocol, Version = version, StatusCode = statusCode };
        }

        public static Headers ParseHeaders(string lines) {
            var headers = new Headers();
            var previousLineWasEmpty = false;
            while (true) {
                var lineBreakIndex = lines.IndexOf(Common.LineSeparator);
                if (previousLineWasEmpty) {
                    if (lineBreakIndex == 0)
                        break;
                    throw new FormatException();
                }
                if (lineBreakIndex == 0) {
                    previousLineWasEmpty = true;
                } else if (lineBreakIndex == -1) {
                    throw new FormatException();
                }
                var line = lines.Substring(0, lineBreakIndex);
                lines = lines.Substring(lineBreakIndex + 2);
                var separatorIndex = line.IndexOf(": ");
                if (separatorIndex == -1)
                    throw new FormatException();
                headers[line.Substring(0, separatorIndex)] = line.Substring(separatorIndex + 2);
            }
            return headers;
        }
    }
}
