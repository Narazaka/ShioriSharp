using System;

[EnumWithValues.EnumWithValues("AAAStruct")]
enum AAA {
    [EnumWithValues.EnumValue("BA", 'a')]
    BBB,
}

namespace EnumTestCli.Foo {
    [EnumWithValues.EnumWithValues("FooStruct")]
    enum Foo {
        [EnumWithValues.EnumValue("BA", 'a')]
        ba,
        [EnumWithValues.EnumValue("BAZ", 'c')]
        baz,
    }
    class Program {
        static void Main(string[] args) {
            FooStruct fooStruct = "BA";
            Console.WriteLine((string)fooStruct);
        }
    }
}
