using MediatR;

namespace KC.Application.Common.Auth
{
    public readonly record struct ValidateDevUserCommand(string Email, string Password)
        : IRequest<ValidateDevUserResponse>;
}
