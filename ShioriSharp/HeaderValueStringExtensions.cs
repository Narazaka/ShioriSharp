using System.Collections.Generic;
using System.Linq;

namespace ShioriSharp {
    public static class HeaderValueStringExtensions {
        public static string[] SeparateValues(this string str, string separator = "\x01") =>
#if NETSTANDARD2_1 || NET5_0
            str.Split(separator);
#else
            str.Split(new string[] { separator }, System.StringSplitOptions.None);
#endif
        public static string CombineValues(this IEnumerable<string?> strs, string separator = "\x01") => string.Join(separator, strs);
        public static IEnumerable<string[]> SeparateValues2(this string str, string separator1 = "\x02", string separator2 = "\x01") =>
#if NETSTANDARD2_1 || NET5_0
            str.Split(separator1).Select(part => part.Split(separator2));
#else
            str.Split(new string[] { separator1 }, System.StringSplitOptions.None).Select(part => part.Split(new string[] { separator2 }, System.StringSplitOptions.None));
#endif
        public static string CombineValues2(this IEnumerable<IEnumerable<string?>> strs, string separator1 = "\x02", string separator2 = "\x01") => string.Join(separator1, strs.Select(part => string.Join(separator2, part)));
    }
}

