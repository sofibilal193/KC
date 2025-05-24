using KC.Utils.Common.Extensions;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace KC.Application.Common.Extensions
{
    public static class ProblemDetailsOptionsExtensions
    {
        public static void Configure(this ProblemDetailsOptions opts, IHostEnvironment env)
        {
            // Control when an exception is included
            opts.IncludeExceptionDetails = (_, _) => env.IsDevelopment() || env.IsTest();

            // map validation exception to badrequest
            opts.Map<ValidationException>(vex =>
            {
                return new ValidationProblemDetails(vex.Errors.GetValidationErrors())
                {
                    // Instance = context.HttpContext.Request.Path,
                    // Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };
            });
        }
    }
}
