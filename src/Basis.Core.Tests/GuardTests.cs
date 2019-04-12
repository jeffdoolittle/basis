using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class GuardTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(1, false)]
        public void can_guard_for_not_null(object value, bool expectException)
        {
            Action action = () => Guard.NotNull(value, nameof(value));

            if (expectException)
            {
                action.Should().Throw<ArgumentNullException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(Messages.caller_does_not_allow_null.Detokenize());
            }
            else
            {
                action.Should().NotThrow<ArgumentNullException>();
            }
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData(" ", true)]
        [InlineData("\t", true)]
        [InlineData("\r", true)]
        [InlineData("\r\n", true)]
        [InlineData("\n", true)]
        [InlineData("*", false)]
        public void can_guard_for_not_null_or_whitespace(string value, bool expectException)
        {
            Action action = () => Guard.NotNullOrWhitespace(value, nameof(value));

            if (expectException)
            {
                action.Should().Throw<ArgumentException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(Messages.caller_does_not_allow_null_or_whitespace.Detokenize());
            }
            else
            {
                action.Should().NotThrow<ArgumentException>();
            }
        }

        [Theory]
        [InlineData(-1, 0, 1, true)]
        [InlineData(1, 2, 3, true)]
        [InlineData(2, 2, 3, true)]
        [InlineData(2, 1, 2, true)]
        [InlineData(2, 0, 4, false)]
        public void can_guard_for_range_with_exclusivity(long value, long min, long max, bool expectException)
        {
            Action action = () => Guard.InRangeExclusive(value, min, max, "value");

            if (expectException)
            {
                action.Should().Throw<ArgumentException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(string.Format(Messages.caller_requires_value_to_be_between_x_and_y, nameof(can_guard_for_range_with_exclusivity), min, max));
            }
            else
            {
                action.Should().NotThrow<ArgumentException>();
            }
        }

        [Theory]
        [InlineData(-1, 0, 1, true)]
        [InlineData(1, 2, 3, true)]
        [InlineData(2, 0, 4, false)]
        [InlineData(1, 1, 1, false)]
        public void can_guard_for_range_with_inclusivity(long value, long min, long max, bool expectException)
        {
            Action action = () => Guard.InRangeInclusive(value, min, max, "value");

            if (expectException)
            {
                action.Should().Throw<ArgumentException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(string.Format(Messages.caller_requires_value_to_be_between_x_and_y_inclusive, nameof(can_guard_for_range_with_inclusivity), min, max));
            }
            else
            {
                action.Should().NotThrow<ArgumentException>();
            }
        }

        [Theory]
        [InlineData(null, 1, 1, true)]
        [InlineData("", 1, 1, true)]
        [InlineData(" ", 2, 2, true)]
        [InlineData(" ", 0, 1, false)]
        public void can_guard_for_length(string value, int min, int max, bool expectException)
        {
            Action action = () => Guard.Length(value, min, max, nameof(value));

            if (expectException)
            {
                action.Should().Throw<ArgumentException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(string.Format(Messages.caller_requires_parameter_length_to_be_between_x_and_y_characters, nameof(can_guard_for_length), min, max));
            }
            else
            {
                action.Should().NotThrow<ArgumentException>();
            }
        }

        [Theory]
        [InlineData(1, 2, 3, false)]
        [InlineData(3, 2, 1, true)]
        public void can_guard_longs_are_sorted_ascending(long a, long b, long c, bool expectException)
        {
            Action action = () => Guard.AreSortedAscending(new[] { a, b, c });

            if (expectException)
            {
                action.Should().Throw<ArgumentException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(string.Format(Messages.caller_expected_specified_values_to_be_in_ascending_sequence, nameof(can_guard_longs_are_sorted_ascending)));
            }
            else
            {
                action.Should().NotThrow<ArgumentException>();
            }
        }

        [Theory]
        [MemberData(nameof(EmptyCollectionSource))]
        public void can_guard_for_not_null_or_empty_collection(IEnumerable<object> collection, bool expectException)
        {
            Action action = () => Guard.NotNullOrEmpty(collection, nameof(collection));

            if (expectException)
            {
                action.Should().Throw<ArgumentException>()
                    .And.Message.Should()
                    .ContainEquivalentOf(string.Format(Messages.caller_expected_at_least_one_item_for_x, nameof(can_guard_for_not_null_or_empty_collection), nameof(collection)));
            }
            else
            {
                action.Should().NotThrow<ArgumentException>();
            }
        }

        public static IEnumerable<object[]> EmptyCollectionSource()
        {
            yield return new object[] { null, true };
            yield return new object[] { new object[0], true };
            yield return new object[] { new List<object>().ToArray(), true };
            yield return new object[] { new[] { new object() }, false };
        }
    }
}
