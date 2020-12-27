using System;

namespace ShioriSharp {
    public static class MethodExtension {
        public static string ToMethodString(this Method method) {
            switch (method) {
                case Method.GET:
                    return "GET";
                case Method.NOTIFY:
                    return "NOTIFY";
                case Method.GET_Version:
                    return "GET Version";
                case Method.GET_Sentence:
                    return "GET Sentence";
                case Method.GET_Word:
                    return "GET Word";
                case Method.GET_Status:
                    return "GET Status";
                case Method.TEACH:
                    return "TEACH";
                case Method.GET_String:
                    return "GET String";
                case Method.NOTIFY_OwnerGhostName:
                    return "NOTIFY OwnerGhostName";
                case Method.NOTIFY_OtherGhostName:
                    return "NOTIFY OtherGhostName";
                case Method.TRANSLATE_Sentence:
                    return "TRANSLATE Sentence";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

