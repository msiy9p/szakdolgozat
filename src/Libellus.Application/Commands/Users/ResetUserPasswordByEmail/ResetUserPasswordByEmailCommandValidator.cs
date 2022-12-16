using FluentValidation;

namespace Libellus.Application.Commands.Users.ResetUserPasswordByEmail;

public sealed class ResetUserPasswordByEmailCommandValidator : AbstractValidator<ResetUserPasswordByEmailCommand>
{
    public ResetUserPasswordByEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.ResetToken)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty();
    }
}