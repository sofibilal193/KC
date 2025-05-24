using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KC.Application.Common.Validations
{
    public class ApiValidationProblemDetails : ValidationProblemDetails
    {
        public string? TraceId { get; set; }

        public Dictionary<string, string[]> ErrorCodes { get; set; } = new();

        public ApiValidationProblemDetails(ModelStateDictionary modelState) : base(modelState)
        {
        }
    }
}
