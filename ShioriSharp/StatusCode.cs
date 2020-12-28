using EnumWithValues;

namespace ShioriSharp {
    /** <summary>SHIORI status code</summary> */
    [EnumWithValues("StatusCode")]
    public enum StatusCodeEnum {
        [EnumValue("OK")]
        OK = 200,
        [EnumValue("No Content")]
        No_Content = 204,
        [EnumValue("Communicate")]
        Communicate = 310,
        [EnumValue("Not Enough")]
        Not_Enough = 311,
        [EnumValue("Advice")]
        Advice = 312,
        [EnumValue("Bad Request")]
        Bad_Request = 400,
        [EnumValue("Internal Server Error")]
        Internal_Server_Error = 500,
    }
}

