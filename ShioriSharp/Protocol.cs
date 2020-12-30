using System;
using EnumWithValues;

namespace ShioriSharp {
    /** <summary>SHIORI protocol</summary> */
    [EnumWithValues("Protocol", convertEnumValue: false)]
    public enum ProtocolEnum : byte {
        [EnumValue("SHIORI")]
        SHIORI = 1,
        [EnumValue("SAORI")]
        SAORI,
    }

    public partial struct Protocol : IValidatable<Protocol> {
        public bool Valid { get => AsEnum != 0; }
        public Protocol Validate() => Valid ? this : throw new System.InvalidOperationException();
    }
}

