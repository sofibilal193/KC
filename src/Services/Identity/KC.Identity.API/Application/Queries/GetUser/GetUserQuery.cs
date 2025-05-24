using FluentValidation;
using MediatR;
using KC.Domain.Common.Types;

namespace KC.Identity.API.Application
{
    public readonly record struct GetUserQuery(int UserId) : IRequest<UserDto?>;

    public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidator()
        {
            RuleFor(v => v.UserId).NotEmpty().GreaterThan(0);
        }
    }
}
