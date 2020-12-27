using System;

namespace ShioriSharp {
    public static class VersionExtension {
        public static string ToVersionString(this Version version) {
            switch (version) {
                case Version.V2_0:
                    return "2.0";
                case Version.V2_2:
                    return "2.2";
                case Version.V2_3:
                    return "2.3";
                case Version.V2_4:
                    return "2.4";
                case Version.V2_5:
                    return "2.5";
                case Version.V2_6:
                    return "2.6";
                case Version.V3_0:
                    return "3.0";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

