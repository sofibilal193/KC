using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace KC.Application.Common.Binders
{
    [ExcludeFromCodeCoverage]
    public class JsonModelBinder : IModelBinder
    {
        private readonly JsonOptions _options;

        public JsonModelBinder(IOptions<JsonOptions> options) =>
            _options = options.Value;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(bindingContext);

                // Test if a value is received
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == ValueProviderResult.None)
                    return Task.CompletedTask;

                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                // Get the json serialized value as string
                string? jsonSerialized = valueProviderResult.FirstValue;

                // Return a successful binding for empty strings or nulls
                if (string.IsNullOrEmpty(jsonSerialized))
                {
                    bindingContext.Result = ModelBindingResult.Success(null);
                    return Task.CompletedTask;
                }

                // Deserialize json string using custom json options defined in startup, if available
                object? deserialized = _options?.JsonSerializerOptions is null ?
                    JsonSerializer.Deserialize(jsonSerialized, bindingContext.ModelType) :
                    JsonSerializer.Deserialize(jsonSerialized, bindingContext.ModelType, _options.JsonSerializerOptions);

                // Run data annotation validation to validate properties and fields on deserialized model
                if (deserialized is not null)
                {
                    var validationResultProps = from property in TypeDescriptor.GetProperties(deserialized).Cast<PropertyDescriptor>()
                                                from attribute in property.Attributes.OfType<ValidationAttribute>()
                                                where !attribute.IsValid(property.GetValue(deserialized))
                                                select new
                                                {
                                                    Member = property.Name,
                                                    ErrorMessage = attribute.FormatErrorMessage(string.Empty)
                                                };

                    var validationResultFields = from field in TypeDescriptor.GetReflectionType(deserialized).GetFields()
                                                 from attribute in field.GetCustomAttributes<ValidationAttribute>()
                                                 where !attribute.IsValid(field.GetValue(deserialized))
                                                 select new
                                                 {
                                                     Member = field.Name,
                                                     ErrorMessage = attribute.FormatErrorMessage(string.Empty)
                                                 };

                    // Add the validation results to the model state
                    var errors = validationResultFields.Concat(validationResultProps);
                    foreach (var validationResultItem in errors)
                        bindingContext.ModelState.AddModelError(validationResultItem.Member, validationResultItem.ErrorMessage);
                }

                // Set successful binding result
                bindingContext.Result = ModelBindingResult.Success(deserialized);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw new ArgumentException("We were unable to process your payload. Please check the payload you are sending and confirm that each field conforms to their respective data type constraints as specified in the schema.");
            }
        }
    }
}
