using System;

namespace ShioriSharp {
    /** <summary>SHIORI Response Message's StatusLine Container</summary> */
    public class StatusLine : IValidatable<StatusLine> {
        public StatusCode StatusCode { get; set; } = StatusCode.OK;
        public Protocol Protocol { get; set; } = Protocol.SHIORI;
        public Version Version { get; set; } = Version.V3_0;

        public override string ToString() => $"{Protocol}/{Version} {(int)StatusCode} {StatusCode}";

        public bool Valid {
            get {
                if (!StatusCode.Valid || !Protocol.Valid || !Version.Valid)
                    return false;
                switch (Protocol.AsEnum) {
                    case Protocol.Enum.SHIORI:
                        return Version >= 2.0 && Version <= 3.0;
                    case Protocol.Enum.SAORI:
                        return Version.AsEnum == VersionEnum.V1_0;
                    default:
                        return false;
                }
            }
        }

        public StatusLine Validate() {
            if (!Valid)
                throw new InvalidOperationException($"[{Protocol}][{Version}][{StatusCode}] is invalid status line");
            return this;
        }
    }
}

