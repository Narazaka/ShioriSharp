using System;

namespace ShioriSharp {
    /** <summary>SHIORI Response Message's StatusLine Container</summary> */
    public class StatusLine {
        public StatusCode StatusCode { get; set; } = StatusCode.OK;
        public Protocol Protocol { get; set; } = Protocol.SHIORI;
        public Version Version { get; set; } = Version.V3_0;

        public override string ToString() => $"{Protocol}/{Version} {(int)StatusCode} {StatusCode}";

        public bool Valid {
            get {
                if (Protocol != Protocol.SHIORI)
                    return false;
#if NET5_0
                if (!Enum.IsDefined(StatusCode.AsEnum))
                    return false;
                if (!Enum.IsDefined(Version.AsEnum))
                    return false;
#else
                if (!Enum.IsDefined(typeof(StatusCode), StatusCode))
                    return false;
                if (!Enum.IsDefined(typeof(Version), Version.AsEnum))
                    return false;
#endif
                return true;
            }
        }

        public StatusLine Validate() {
            if (!Valid)
                throw new InvalidOperationException($"[{Protocol}][{Version}][{StatusCode}] is invalid status line");
            return this;
        }
    }
}

