using FluentValidation;
using KC.Application.Common.Validations;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Extensions
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> UniqueOrgCode<T>(this IRuleBuilder<T, string> ruleBuilder, IIdentityUnitOfWork data)
        {
            return ruleBuilder.MustAsync(async (code, cancellationToken) => !await data.Orgs.AnyAsync(o => o.Code == code, cancellationToken))
                .WithErrorCode(ValidationCodes.UniqueOrgCodeValidator);
        }
    }
}
