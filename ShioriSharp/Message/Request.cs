using System;

namespace ShioriSharp.Message {
    public class Request : IValidatable<Request> {
        public Request() { }
        public Request(Method method) => Method = method;

        public RequestLine RequestLine { get; set; } = new();
        public Headers Headers { get; set; } = new();

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
