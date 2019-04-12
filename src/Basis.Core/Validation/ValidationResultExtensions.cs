using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Basis.Validation
{
    public static class ValidationResultExtensions
    {
        public static bool HasErrors(this IEnumerable<ValidationResult> results)
        {
            return results.Any();
        }

        public static void ThrowIfInvalid(this IEnumerable<ValidationResult> results)
        {
            var validationResults = results as ValidationResult[] ?? results.ToArray();

            if (validationResults.HasErrors())
            {
                throw new ValidationResultException(validationResults);
            }
        }
    }
}