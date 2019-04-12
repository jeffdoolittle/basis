using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Basis.Validation
{
    [Serializable]
    public class ValidationResultException : BasisException
    {
        public ValidationResultException(ICollection<ValidationResult> results)
            : base($"A validation result exception occurred.{Environment.NewLine}{string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage))}")
        {
            Results = results;
        }

        public IEnumerable<ValidationResult> Results { get; }
    }
}