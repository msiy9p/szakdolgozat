using FluentValidation;

namespace Libellus.Application.Commands.Users.ChangeUserEmail;

public sealed class ChangeUserEmailCommandValidator : AbstractValidator<ChangeUserEmailCommand>
{
    public ChangeUserEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}