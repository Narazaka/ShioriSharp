using System;

namespace ShioriSharp {
    public static class ProtocolExtension {
        public static string ToProtocolString(this Protocol protocol) {
            switch (protocol) {
                case Protocol.SHIORI:
                    return "SHIORI";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

