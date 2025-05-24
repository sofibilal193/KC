using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace KC.Utils.Common.Extensions
{
    public static class ValidationFailureExtensions
    {
        public static IDictionary<string, string[]> GetValidationErrors(this IEnumerable<ValidationFailure> failures)
        {
            return failures.GroupBy(g => g.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
        }
    }
}
