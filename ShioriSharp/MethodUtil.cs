using System;

namespace ShioriSharp {
    public static class MethodUtil {
        public static Method ToMethodEnum(string methodString) {
            switch (methodString) {
                case "GET":
                    return Method.GET;
                case "NOTIFY":
                    return Method.NOTIFY;
                case "GET Version":
                    return Method.GET_Version;
                case "GET Sentence":
                    return Method.GET_Sentence;
                case "GET Word":
                    return Method.GET_Word;
                case "GET Status":
                    return Method.GET_Status;
                case "TEACH":
                    return Method.TEACH;
                case "GET String":
                    return Method.GET_String;
                case "NOTIFY OwnerGhostName":
                    return Method.NOTIFY_OwnerGhostName;
                case "NOTIFY OtherGhostName":
                    return Method.NOTIFY_OtherGhostName;
                case "TRANSLATE Sentence":
                    return Method.TRANSLATE_Sentence;
                default:
                    throw new FormatException($"unknown method [{methodString}]");
            }
        }
    }
}

