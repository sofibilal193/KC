using FluentValidation;
using KC.Application.Common.Extensions;
using KC.Application.Common.Validations;

namespace KC.Identity.API.Application
{
    public class GetAllUsersByPermissionsQueryValidator : AbstractValidator<GetAllUsersByPermissionsQuery>
    {
        public GetAllUsersByPermissionsQueryValidator()
        {
            RuleFor(x => x.Permissions).NotEmpty();
        }
    }
}