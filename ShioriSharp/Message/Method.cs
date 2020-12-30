using EnumWithValues;

namespace ShioriSharp.Message {
    /** <summary>SHIORI/2.x/3.x method</summary> */
    [EnumWithValues("Method", convertEnumValue: false)]
    public enum MethodEnum : byte {
        [EnumValue("GET")]
        GET = 1,
        [EnumValue("NOTIFY")]
        NOTIFY,
        [EnumValue("GET Version")]
        GET_Version,
        [EnumValue("GET Sentence")]
        GET_Sentence,
        [EnumValue("GET Word")]
        GET_Word,
        [EnumValue("GET Status")]
        GET_Status,
        [EnumValue("TEACH")]
        TEACH,
        [EnumValue("GET String")]
        GET_String,
        [EnumValue("NOTIFY OwnerGhostName")]
        NOTIFY_OwnerGhostName,
        [EnumValue("NOTIFY OtherGhostName")]
        NOTIFY_OtherGhostName,
        [EnumValue("TRANSLATE Sentence")]
        TRANSLATE_Sentence,
        [EnumValue("EXECUTE")]
        EXECUTE,
    }

    public partial struct Method : IValidatable<Method> {
        public bool Valid { get => AsEnum != 0; }
        public Method Validate() => Valid ? this : throw new System.InvalidOperationException();
    }
}

