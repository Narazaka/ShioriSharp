using System;

namespace ShioriSharp {
    public static class StatusCodeExtension {
        public static string ToStatusCodeString(this StatusCode statusCode) {
            switch (statusCode) {
                case StatusCode.OK: return "OK";
                case StatusCode.No_Content: return "No Content";
                case StatusCode.Communicate: return "Communicate";
                case StatusCode.Not_Enough: return "Not Enough";
                case StatusCode.Advice: return "Advice";
                case StatusCode.Bad_Request: return "Bad Request";
                case StatusCode.Internal_Server_Error: return "Internal Server Error";
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}

