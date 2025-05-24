using FluentValidation;
using KC.Application.Common.Auth;

namespace KC.Identity.API.Application
{
    public class ValidateDevUserCommandValidator : AbstractValidator<ValidateDevUserCommand>
    {
        public ValidateDevUserCommandValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty()
                .MaximumLength(100)
                .EmailAddress();
            RuleFor(v => v.Password)
                .NotEmpty()
                .MaximumLength(25);
        }
    }
}
