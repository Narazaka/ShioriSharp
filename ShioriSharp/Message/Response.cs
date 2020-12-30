using System;

namespace ShioriSharp.Message {
    public class Response : IValidatable<Response> {
        public Response() { }
        public Response(int statusCode) => StatusCode = statusCode;
        public Response(int statusCode, Protocol protocol) : this(statusCode) {
            Protocol = protocol;
            if (protocol.AsEnum == ProtocolEnum.SAORI)
                Version = VersionEnum.V1_0;
        }

        public StatusLine StatusLine { get; set; } = new();
        public Headers Headers { get; set; } = new();

        public StatusCode StatusCode { get => StatusLine.StatusCode; set => StatusLine.StatusCode = value; }
        public Protocol Protocol { get => StatusLine.Protocol; set => StatusLine.Protocol = value; }
        public Version Version { get => StatusLine.Version; set => StatusLine.Version = value; }


        public override string ToString() => $"{StatusLine}{Common.LineSeparator}{Headers}{Common.LineSeparator}";

        public bool Valid { get => StatusLine.Valid && Headers.Valid; }

        public Response Validate() {
            if (!Valid)
                throw new InvalidOperationException($"invalid response");
            return this;
        }
    }

}

