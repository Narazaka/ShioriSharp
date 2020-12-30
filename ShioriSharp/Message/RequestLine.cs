using System;

namespace ShioriSharp.Message {
    /** <summary>SHIORI Request Message's RequestLine Container</summary> */
    public class RequestLine : IValidatable<RequestLine> {
        public Method Method { get; set; } = Method.GET;
        public Protocol Protocol { get; set; } = Protocol.SHIORI;
        public Version Version { get; set; } = Version.V3_0;

        public override string ToString() => $"{Method} {Protocol}/{Version}";

        public bool Valid {
            get {
                if (Protocol == Protocol.SHIORI) {
                    switch (Version.AsEnum) {
                        case Version.Enum.V2_0:
                            switch (Method.AsEnum) {
                                case Method.Enum.GET_Version:
                                case Method.Enum.NOTIFY_OwnerGhostName:
                                case Method.Enum.GET_Sentence:
                                case Method.Enum.GET_Word:
                                case Method.Enum.GET_Status:
                                    return true;
                                default:
                                    return false;
                            }
                        case Version.Enum.V2_2:
                            switch (Method.AsEnum) {
                                case Method.Enum.GET_Sentence:
                                    return true;
                                default:
                                    return false;
                            }
                        case Version.Enum.V2_3:
                            switch (Method.AsEnum) {
                                case Method.Enum.NOTIFY_OtherGhostName:
                                case Method.Enum.GET_Sentence:
                                    return true;
                                default:
                                    return false;
                            }
                        case Version.Enum.V2_4:
                            switch (Method.AsEnum) {
                                case Method.Enum.TEACH:
                                    return true;
                                default:
                                    return false;
                            }
                        case Version.Enum.V2_5:
                            switch (Method.AsEnum) {
                                case Method.Enum.GET_String:
                                    return true;
                                default:
                                    return false;
                            }
                        case Version.Enum.V2_6:
                            switch (Method.AsEnum) {
                                case Method.Enum.GET_Version:
                                case Method.Enum.GET_Sentence:
                                case Method.Enum.GET_Status:
                                case Method.Enum.GET_String:
                                case Method.Enum.NOTIFY_OwnerGhostName:
                                case Method.Enum.NOTIFY_OtherGhostName:
                                case Method.Enum.TRANSLATE_Sentence:
                                    return true;
                                default:
                                    return false;
                            }
                        case Version.Enum.V3_0:
                            switch (Method.AsEnum) {
                                case Method.Enum.GET:
                                case Method.Enum.NOTIFY:
                                    return true;
                                default:
                                    return false;
                            }
                        default:
                            return false;
                    }
                } else if (Protocol == Protocol.SAORI) {
                    if (Version != Version.V1_0)
                        return false;
                    switch (Method.AsEnum) {
                        case MethodEnum.GET_Version:
                        case MethodEnum.EXECUTE:
                            return true;
                        default:
                            return false;
                    }
                } else {
                    return false;
                }
            }
        }

        public RequestLine Validate() {
            if (!Valid)
                throw new InvalidOperationException($"[{Method}][{Protocol}][{Version}] is invalid request line");
            return this;
        }
    }
}

