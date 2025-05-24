using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace KC.Application.Common.Filters
{
    public class AddSessionIdHeaderProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Session-Id",
                Kind = OpenApiParameterKind.Header,
                Schema = new JsonSchema { Type = JsonObjectType.String },
                IsRequired = false,
                Description = "An optional unique identifier for the transaction or user's session to help with troubleshooting.",
            });
            return true;
        }
    }
}
