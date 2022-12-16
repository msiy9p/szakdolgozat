using FluentValidation;

namespace Libellus.Application.Commands.Users.GenerateUserPasswordResetToken;

public sealed class GenerateUserPasswordResetTokenCommandValidator : AbstractValidator<GenerateUserPasswordResetTokenCommand>
{
    public GenerateUserPasswordResetTokenCommandValidator()
    {
        RuleFor(x => x.ForgotPasswordCallbackUrlTemplate)
            .NotNull();
    }
}