using System;

namespace ShioriSharp {
    /** <summary>SHIORI Request Message's RequestLine Container</summary> */
    public class RequestLine {
        public Method Method { get; set; } = Method.GET;
        public Protocol Protocol { get; set; } = Protocol.SHIORI;
        public Version Version { get; set; } = Version.V3_0;

        public RequestLine() { }

        public RequestLine(Method method = Method.GET, Protocol protocol = Protocol.SHIORI, Version version = Version.V3_0, bool validate = false) {
            Method = method;
            Protocol = protocol;
            Version = version;
            if (validate)
                Validate();
        }

        public override string ToString() => $"{Method.ToMethodString()} {Protocol.ToProtocolString()}/{Version.ToVersionString()}";

        public bool Valid {
            get {
                if (Protocol != Protocol.SHIORI)
                    return false;
                switch (Version) {
                    case Version.V2_0:
                        switch (Method) {
                            case Method.GET_Version:
                            case Method.NOTIFY_OwnerGhostName:
                            case Method.GET_Sentence:
                            case Method.GET_Word:
                            case Method.GET_Status:
                                return true;
                            default:
                                return false;
                        }
                    case Version.V2_2:
                        switch (Method) {
                            case Method.GET_Sentence:
                                return true;
                            default:
                                return false;
                        }
                    case Version.V2_3:
                        switch (Method) {
                            case Method.NOTIFY_OtherGhostName:
                            case Method.GET_Sentence:
                                return true;
                            default:
                                return false;
                        }
                    case Version.V2_4:
                        switch (Method) {
                            case Method.TEACH:
                                return true;
                            default:
                                return false;
                        }
                    case Version.V2_5:
                        switch (Method) {
                            case Method.GET_String:
                                return true;
                            default:
                                return false;
                        }
                    case Version.V2_6:
                        switch (Method) {
                            case Method.GET_Version:
                            case Method.GET_Sentence:
                            case Method.GET_Status:
                            case Method.GET_String:
                            case Method.NOTIFY_OwnerGhostName:
                            case Method.NOTIFY_OtherGhostName:
                            case Method.TRANSLATE_Sentence:
                                return true;
                            default:
                                return false;
                        }
                    case Version.V3_0:
                        switch (Method) {
                            case Method.GET:
                            case Method.NOTIFY:
                                return true;
                            default:
                                return false;
                        }
                    default:
                        return false;
                }
            }
        }

        public void Validate() {
            if (!Valid)
                throw new InvalidOperationException($"[{Method}][{Protocol}][{Version}] is invalid request line");
        }
    }
}

