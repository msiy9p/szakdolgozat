using FluentValidation;

namespace Libellus.Application.Commands.Users.ConfirmUserEmail;

public sealed class ConfirmUserEmailCommandValidator : AbstractValidator<ConfirmUserEmailCommand>
{
    public ConfirmUserEmailCommandValidator()
    {
        RuleFor(x => x.EmailToken)
            .NotEmpty();
    }
}