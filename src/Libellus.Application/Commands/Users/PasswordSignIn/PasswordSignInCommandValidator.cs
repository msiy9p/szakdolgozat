using FluentValidation;

namespace Libellus.Application.Commands.Users.PasswordSignIn;

public sealed class PasswordSignInCommandValidator : AbstractValidator<PasswordSignInCommand>
{
    public PasswordSignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}