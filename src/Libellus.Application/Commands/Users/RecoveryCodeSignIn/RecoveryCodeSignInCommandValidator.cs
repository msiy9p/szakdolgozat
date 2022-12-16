using FluentValidation;

namespace Libellus.Application.Commands.Users.RecoveryCodeSignIn;

public sealed class RecoveryCodeSignInCommandValidator : AbstractValidator<RecoveryCodeSignInCommand>
{
    public RecoveryCodeSignInCommandValidator()
    {
        RuleFor(x => x.RecoveryCode)
            .NotEmpty();
    }
}