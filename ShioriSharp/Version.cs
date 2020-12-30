using EnumWithValues;

namespace ShioriSharp {
    /** <summary>SHIORI version</summary> */
    [EnumWithValues("Version", convertEnumValue: false)]
    public enum VersionEnum {
        [EnumValue("1.0", 1.0)]
        V1_0 = 10,
        [EnumValue("2.0", 2.0)]
        V2_0 = 20,
        [EnumValue("2.2", 2.2)]
        V2_2 = 22,
        [EnumValue("2.3", 2.3)]
        V2_3 = 23,
        [EnumValue("2.4", 2.4)]
        V2_4 = 24,
        [EnumValue("2.5", 2.5)]
        V2_5 = 25,
        [EnumValue("2.6", 2.6)]
        V2_6 = 26,
        [EnumValue("3.0", 3.0)]
        V3_0 = 30,
    }
}

