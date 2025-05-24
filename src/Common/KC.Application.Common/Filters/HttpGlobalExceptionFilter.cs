using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FluentValidation;
using KC.Application.Common.Validations;
using KC.Domain.Common.Exceptions;
using System;
using System.Net.Http;

namespace KC.Application.Common.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ApiBehaviorOptions _options;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IOptions<ApiBehaviorOptions> options, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception, "");

            switch (context.Exception)
            {
                case NotFoundException:
                    context.Result = new NotFoundResult();
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ArgumentException:
                    context.Result = new BadRequestObjectResult("We were unable to process your payload. Please check the payload you are sending and confirm that each field conforms to their respective data type constraints as specified in the schema.");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ValidationException exception:
                    foreach (var error in exception.Errors)
                    {
                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                    var details = new ApiValidationProblemDetails(context.ModelState);
                    if (_options.ClientErrorMapping.TryGetValue((int)HttpStatusCode.BadRequest, out var data))
                    {
                        details.Title ??= data.Title;
                        details.Type ??= data.Link;
                    }
                    foreach (var property in exception.Errors.Select(e => e.PropertyName).Distinct())
                    {
                        details.ErrorCodes.Add(property, exception.Errors.Where(e => e.PropertyName == property).Select(e => e.ErrorCode).ToArray());
                    }
                    details.TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                    context.Result = new BadRequestObjectResult(details);
                    context.ExceptionHandled = true;
                    break;
                case HttpRequestException reqException:
                    if (reqException.StatusCode == HttpStatusCode.Forbidden)
                    {
                        context.Result = new ForbidResult();
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                    break;
            }
        }
    }
}
