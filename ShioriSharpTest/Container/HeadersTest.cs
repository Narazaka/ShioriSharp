using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ShioriSharp;
using ShioriSharp.Message;

namespace ShioriSharpTest.Container {
    public class HeadersTest {
        [Theory]
        [InlineData("Charset", "Shift_JIS")]
        [InlineData("Foo:Bar", "Baz: Piyo")]
        [InlineData("Foo Bar", "Baz: Piyo")]
        [InlineData("Foo\x01\x02 Bar", "Baz\x01\x02Piyo")]
        public void Valid(string key, string value) {
            var instance = new Headers { { key, value } };
            Assert.True(instance.Valid);
            Assert.True(instance.Validate() != null);
        }

        [Fact]
        public void ValidExtra() { // xunit cannot handle this inline data
            var instance = new Headers { { "Foo", "Baz!\"#$%&'()-=^~\\|[{]}@`:*;+_/?.>,<" } };
            Assert.True(instance.Valid);
            Assert.True(instance.Validate() != null);
        }

        [Theory]
        [InlineData("Charset", "Charset", "UTF-8\n")]
        [InlineData("Foo: Bar", "Foo: Bar", "Baz")]
        [InlineData("Baz", "Foo", "Bar", "Baz", "Hoge\n")]
        public void Invalid(string errorKey, params string[] values) {
            var instance = ValuesToHeaders(values);
            Assert.False(instance.Valid);
            Assert.Equal(instance.InvalidPair!.Value.Key, errorKey);
            Assert.Throws<InvalidOperationException>(() => instance.Validate());
        }

        [Theory]
        [InlineData("Charset: UTF-8\x0d\x0aReference0: Foo\x01/Bar\x0d\x0a", "Charset", "UTF-8", "Reference0", "Foo\x01/Bar")]
        public void ToStringTest(string result, params string[] values) {
            var instance = ValuesToHeaders(values);
            Assert.Equal(instance.ToString(), result);
            Assert.Equal($"{instance}", result);
        }

        [Theory]
        [InlineData("Charset", "Shift_JIS")]
        [InlineData("Foo:Bar", "Baz: Piyo")]
        [InlineData("Foo Bar", "Baz: Piyo")]
        [InlineData("Foo\x01\x02 Bar", "Baz\x01\x02Piyo")]
        public void Get(string key, string value) {
            var instance = new Headers { { key, value } };
            Assert.Equal(instance.Get(key), value);
        }

        [Fact]
        public void GetEmpty() {
            Assert.Null(new Headers().Get("Foo"));
            Assert.Null(new Headers().Reference1);
            Assert.Null(new Headers().Reference(10));
        }

        [Fact]
        public void GetKnown() {
            var headers = new Headers { { "Charset", "UTF-8" }, { "Reference1", "Foo" } };
            Assert.Equal("UTF-8", headers.Charset);
            Assert.Equal("Foo", headers.Reference1);
            Assert.Equal("Foo", headers.Reference(1));
        }

        [Theory]
        [InlineData(1, "Reference0", "foo")]
        [InlineData(3, "Reference2", "foo")]
        [InlineData(102, "Reference101", "foo")]
        public void References(int count, params string[] values) {
            var instance = ValuesToHeaders(values);
            var references = instance.References();
            Assert.Equal(count, references.Length);
            Assert.Equal(values.Last(), references[references.Length - 1]);
        }

        [Theory]
        [InlineData("Charset", "Shift_JIS")]
        [InlineData("Foo:Bar", "Baz: Piyo")]
        [InlineData("Foo Bar", "Baz: Piyo")]
        [InlineData("Foo\x01\x02 Bar", "Baz\x01\x02Piyo")]
        public void Set(string key, string value) {
            var instance = new Headers().Set(key, value);
            Assert.Equal(instance[key], value);
        }

        [Fact]
        public void SetKnown() {
            var headers = new Headers { Charset = "UTF-8" , Reference1 = "Foo" };
            headers.Reference(10, "Bar");
            Assert.Equal("UTF-8", headers["Charset"]);
            Assert.Equal("Foo", headers["Reference1"]);
            Assert.Equal("Bar", headers["Reference10"]);
        }

        Headers ValuesToHeaders(string[] values) => new Headers(
            values
            .Select((value, index) => (value, index))
            .GroupBy(tuple => tuple.index / 2, tuple => tuple.value)
            .ToDictionary(group => group.First(), group => group.Last())
            );
    }
}

