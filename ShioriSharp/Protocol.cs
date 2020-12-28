using System;
using EnumWithValues;

namespace ShioriSharp {
    /** <summary>SHIORI protocol</summary> */
    [EnumWithValues("Protocol", convertEnumValue: false)]
    public enum ProtocolEnum : byte {
        [EnumValue("SHIORI")]
        SHIORI = 1,
    }
}

