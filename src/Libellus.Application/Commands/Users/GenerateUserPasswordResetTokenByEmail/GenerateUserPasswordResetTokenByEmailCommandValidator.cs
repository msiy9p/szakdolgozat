using FluentValidation;

namespace Libellus.Application.Commands.Users.GenerateUserPasswordResetTokenByEmail;

public sealed class GenerateUserPasswordResetTokenByEmailCommandValidator : AbstractValidator<GenerateUserPasswordResetTokenByEmailCommand>
{
    public GenerateUserPasswordResetTokenByEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.ForgotPasswordCallbackUrlTemplate)
            .NotNull();
    }
}