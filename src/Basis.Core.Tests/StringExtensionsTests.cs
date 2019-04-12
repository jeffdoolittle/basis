using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("{0}", "")]
        [InlineData(" {0} ", "  ")]
        [InlineData(" {0} {1} ", "   ")]
        [InlineData(" {0}foo{1} ", " foo ")]
        [InlineData(" {0} foo {1} ", "  foo  ")]
        [InlineData(" {0} foo {1} bar ", "  foo  bar ")]
        [InlineData(" {1} foo {0} bar ", "  foo  bar ")]
        public void can_detokenize_format_string(string value, string expected)
        {
            var detokenized = value.Detokenize();
            detokenized.Should().Be(expected);
        }
    }
}