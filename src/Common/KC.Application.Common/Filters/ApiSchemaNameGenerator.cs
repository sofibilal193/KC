using System;
using System.Linq;
using NJsonSchema.Generation;

namespace KC.Application.Common
{
    public class ApiSchemaNameGenerator : ISchemaNameGenerator
    {
        public string Generate(Type type)
        {
            // Remove "Dto" from class names
            if (type.IsGenericType)
            {
                var genericType = type.Name.Split('`')[0];
                var arguments = string.Join(", ", type.GenericTypeArguments.Select(t => t.Name.Replace("Dto", "")));
                return $"{genericType}<{arguments}>"; // NSwag will convert this to GenericTypeOfArguments (e.g. PagedListOfOrg)
            }
            else
            {
                return type.Name.Replace("Dto", "");
            }
        }
    }
}
