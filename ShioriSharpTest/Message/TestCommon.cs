using System.Linq;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public static class TestCommon {
        public static Headers ValuesToHeaders(string[] values) => new Headers(
            values
            .Select((value, index) => (value, index))
            .GroupBy(tuple => tuple.index / 2, tuple => tuple.value)
            .ToDictionary(group => group.First(), group => group.Last())
            );
    }
}
