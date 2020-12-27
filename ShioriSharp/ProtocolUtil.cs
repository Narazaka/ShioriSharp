using System;

namespace ShioriSharp {
    public static class ProtocolUtil {
        public static Protocol ToProtocolEnum(string protocolString) {
            switch (protocolString) {
                case "SHIORI":
                    return Protocol.SHIORI;
                default:
                    throw new FormatException($"unknown protocol [{protocolString}]");
            }
        }
    }
}

