using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basis.Validation
{
    public interface IValidationEngine
    {
        IReadOnlyList<ValidationResult> Validate(object value);
    }
}