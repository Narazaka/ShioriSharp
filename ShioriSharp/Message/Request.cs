using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShioriSharp.Message {
    public class Request : IValidatable<Request> {
        public static Request GET(Headers? headers = null) => new Request { Headers = headers ?? new() };
        public static Request NOTIFY(Headers? headers = null) => new Request { Method = Method.NOTIFY, Headers = headers ?? new() };
#if NET5_0
        public RequestLine RequestLine { get; init; } = new();
        public Headers Headers { get; init; } = new();
#else
        public RequestLine RequestLine { get; set; } = new();
        public Headers Headers { get; set; } = new();
#endif

        public Method Method { get => RequestLine.Method; set => RequestLine.Method = value; }
        public Protocol Protocol { get => RequestLine.Protocol; set => RequestLine.Protocol = value; }
        public Version Version { get => RequestLine.Version; set => RequestLine.Version = value; }

        public override string ToString() => $"{RequestLine}{Common.LineSeparator}{Headers}{Common.LineSeparator}";

        public bool Valid { get => RequestLine.Valid && Headers.Valid; }

        public Request Validate() {
            if (!Valid)
                throw new InvalidOperationException($"invalid request");
            return this;
        }
    }
}
