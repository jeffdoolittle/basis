using System.Linq;

namespace Basis
{
    public static class StringExtensions
    {
        public static string Detokenize(this string value)
        {
            Guard.NotNull(value, nameof(value));

            var replacements = value.Where(x => x == '{').Select(x => "").ToArray();

            return string.Format(value, replacements);
        }
    }
}