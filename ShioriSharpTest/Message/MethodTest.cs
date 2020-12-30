using System;
using Xunit;
using ShioriSharp;
using ShioriSharp.Message;

namespace ShioriSharpTest.Message {
    public class MethodTest {
        [Theory]
        [InlineData("GET")]
        [InlineData("NOTIFY")]
        [InlineData("GET Version")]
        [InlineData("GET Sentence")]
        [InlineData("GET Word")]
        [InlineData("GET Status")]
        [InlineData("TEACH")]
        [InlineData("GET String")]
        [InlineData("NOTIFY OwnerGhostName")]
        [InlineData("NOTIFY OtherGhostName")]
        [InlineData("TRANSLATE Sentence")]
        [InlineData("EXECUTE")]
        public void Valid(string value) {
            Assert.True(((Method)value).Valid);
            Assert.IsType<Method>(((Method)value).Validate());
        }

        [Theory]
        [InlineData("")]
        [InlineData("Foo")]
        [InlineData(null)]
        public void Invalid(string value) {
            Assert.False(((Method)value).Valid);
            Assert.Throws<InvalidOperationException>(() => ((Method)value).Validate());
        }
    }
}

