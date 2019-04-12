using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Basis
{
    public static class Guard
    {
        public static void NotNull(object value, string paramName, [CallerMemberName] string caller = "")
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, string.Format(Messages.caller_does_not_allow_null, caller));
            }
        }

        public static void NotNullOrWhitespace(string value, string paramName, [CallerMemberName] string caller = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(string.Format(Messages.caller_does_not_allow_null_or_whitespace, caller), paramName);
            }
        }

        public static void Length(string value, long min, long max, string paramName, [CallerMemberName] string caller = "")
        {
            Guard.AreSortedAscending(new[] { min, max }, caller);

            var length = (value ?? "").Length;

            if (length < min || length > max)
            {
                throw new ArgumentException(string.Format(Messages.caller_requires_parameter_length_to_be_between_x_and_y_characters, caller, min, max), paramName);
            }
        }

        public static void InRangeExclusive(long value, long min, long max, string paramName, [CallerMemberName] string caller = "")
        {
            Guard.AreSortedAscending(new []{ min, max }, caller);

            if (value < min + 1 || value > max - 1)
            {
                throw new ArgumentException(string.Format(Messages.caller_requires_value_to_be_between_x_and_y, caller, min, max), paramName);
            }
        }

        public static void InRangeInclusive(long value, long min, long max, string paramName, [CallerMemberName] string caller = "")
        {
            Guard.AreSortedAscending(new []{ min, max }, caller);

            if (value < min || value > max)
            {
                throw new ArgumentException(string.Format(Messages.caller_requires_value_to_be_between_x_and_y_inclusive, caller, min, max), paramName);
            }
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> enumerable, string parameterName, [CallerMemberName] string caller = "")
        {
            if (enumerable == null || !enumerable.Any())
            {
                throw new ArgumentException(string.Format(Messages.caller_expected_at_least_one_item_for_x, caller, parameterName), parameterName);
            }
        }

        public static void AreSortedAscending(long[] values, [CallerMemberName] string caller = "")
        {
            Guard.NotNull(values, nameof(values));

            var last = long.MinValue;

            foreach (var t in values)
            {
                if (last > t)
                {
                    throw new ArgumentException(string.Format(Messages.caller_expected_specified_values_to_be_in_ascending_sequence, caller));
                }

                last = t;
            }
        }
    }
}