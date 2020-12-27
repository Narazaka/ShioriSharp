using System;

namespace ShioriSharp {
    /** <summary>SHIORI Response Message's StatusLine Container</summary> */
    public class StatusLine {
        public StatusCode StatusCode { get; set; } = StatusCode.OK;
        public Protocol Protocol { get; set; } = Protocol.SHIORI;
        public Version Version { get; set; } = Version.V3_0;

        public StatusLine() { }

        public StatusLine(StatusCode statusCode = StatusCode.OK, Protocol protocol = Protocol.SHIORI, Version version = Version.V3_0, bool validate = false) {
            StatusCode = statusCode;
            Protocol = protocol;
            Version = version;
            if (validate)
                Validate();
        }

        public override string ToString() => $"{Protocol.ToProtocolString()}/{Version.ToVersionString()} {(int)StatusCode} {StatusCode.ToStatusCodeString()}";

        public bool Valid {
            get {
                if (Protocol != Protocol.SHIORI)
                    return false;
#if NET5_0
                if (!Enum.IsDefined(StatusCode))
                    return false;
                if (!Enum.IsDefined(Version))
                    return false;
#else
                if (!Enum.IsDefined(typeof(StatusCode), StatusCode))
                    return false;
                if (!Enum.IsDefined(typeof(Version), Version))
                    return false;
#endif
                return true;
            }
        }

        public void Validate() {
            if (!Valid)
                throw new InvalidOperationException($"[{Protocol}][{Version}][{StatusCode}] is invalid status line");
        }
    }
}

