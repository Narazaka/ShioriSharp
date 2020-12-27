using System;

namespace ShioriSharp {
    public static class VersionUtil {
        public static Version ToVersionEnum(string versionString) {
            switch (versionString) {
                case "2.0":
                    return Version.V2_0;
                case "2.2":
                    return Version.V2_2;
                case "2.3":
                    return Version.V2_3;
                case "2.4":
                    return Version.V2_4;
                case "2.5":
                    return Version.V2_5;
                case "2.6":
                    return Version.V2_6;
                case "3.0":
                    return Version.V3_0;
                default:
                    throw new FormatException($"unknown version [{versionString}]");
            }
        }
    }
}

