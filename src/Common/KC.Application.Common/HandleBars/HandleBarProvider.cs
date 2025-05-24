using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using HandlebarsDotNet;
using KC.Domain.Common.Exceptions;

namespace KC.Application.Common.HandleBars
{
    [ExcludeFromCodeCoverage]
    public class HandleBarProvider : IHandleBarProvider
    {
        public string GetHtmlReplacedPlaceHolders(string inputHtml, object data, string dataType)
        {
            var type = Type.GetType(dataType)
                ?? throw new DomainException($"Type '{dataType}' could not be found.");
            var template = Handlebars.Compile(inputHtml);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var json = JsonSerializer.Serialize(data);
            var templateData = JsonSerializer.Deserialize(json, type, options);
            return template(templateData);
        }
    }
}
