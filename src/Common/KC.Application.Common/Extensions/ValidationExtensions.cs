using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentValidation;
using KC.Application.Common.Validations;
using KC.Domain.Common;
using KC.Domain.Common.Constants;
using KC.Domain.Common.ApiServices;

namespace KC.Application.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, string?> PhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.PhoneNumber).WithErrorCode(ValidationCodes.PhoneNumberValidator);
        }

        public static IRuleBuilderOptions<T, string?> ZipCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.ZipCode).WithErrorCode(ValidationCodes.ZipCodeValidator);
        }

        public static IRuleBuilderOptions<T, string?> EmailAddress<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.Email).WithErrorCode(ValidationCodes.EmailAddressValidator);
        }

        public static IRuleBuilderOptions<T, string?> Url<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.Url).WithErrorCode(ValidationCodes.UrlValidator);
        }

        public static IRuleBuilderOptions<T, string?> SocalSecurityNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.SocialSecurityNumber).WithErrorCode(ValidationCodes.SocialSecurityValidator);
        }

        public static IRuleBuilderOptions<T, string?> FederalTaxId<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.FederalTaxId).WithErrorCode(ValidationCodes.FederalTaxIdValidator);
        }

        public static IRuleBuilderOptions<T, string?> PersonName<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.PersonName).WithErrorCode(ValidationCodes.PersonNameValidator);
        }

        public static IRuleBuilderOptions<T, string?> Base64String<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Must(s => s == null || Convert.TryFromBase64String(s, new Span<byte>(new byte[s.Length]), out var _))
                .WithErrorCode(ValidationCodes.Base64String);
        }

        public static IRuleBuilderOptions<T, string?> IsValidConfigItem<T>(this IRuleBuilder<T, string?> ruleBuilder,
            IApiService apiService, string configType)
        {
            return ruleBuilder.MustAsync(async (string? value, CancellationToken ct) =>
            {
                if (value is null) return true;
                try
                {
                    var config = await apiService.GetAsync<List<ConfigItemDto>>(
                        ApiServiceTypes.Configs, $"configs/items/{configType}", ct);
                    return config?.Select(d => d.Value?.ToLower())?.Contains(value?.ToLower()) ?? false;
                }
                catch
                {
                    return true;
                }
            }).WithErrorCode(ValidationCodes.EnumValidator);
        }

        public static IRuleBuilderOptions<T, string?> UsState<T>(this IRuleBuilder<T, string?> ruleBuilder, IApiService apiService)
        {
            return ruleBuilder.IsValidConfigItem(apiService, "US_States");
        }

        public static IRuleBuilderOptions<T, string?> AlphabeticCharactersOnly<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(RegularExpressions.AlphabeticCharacters).WithErrorCode(ValidationCodes.AlphabeticValidator);
        }
    }
}
