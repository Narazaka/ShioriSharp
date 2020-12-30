using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShioriSharp.Message {
    public class Response : IValidatable<Response> {
        public static Response OK(Headers? headers = null, double version = 3.0) => new Response { Headers = headers ?? new(), Version = version };
        public static Response NoContent(Headers? headers = null, double version = 3.0) => new Response { StatusCode = StatusCode.No_Content, Headers = headers ?? new(), Version = version };

#if NET5_0
        public StatusLine StatusLine { get; init; } = new();
        public Headers Headers { get; init; } = new();
#else
        public StatusLine StatusLine { get; set; } = new();
        public Headers Headers { get; set; } = new();
#endif

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

