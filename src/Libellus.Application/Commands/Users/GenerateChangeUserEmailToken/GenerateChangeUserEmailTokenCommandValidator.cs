using FluentValidation;

namespace Libellus.Application.Commands.Users.GenerateChangeUserEmailToken;

public sealed class GenerateChangeUserEmailTokenCommandValidator : AbstractValidator<GenerateChangeUserEmailTokenCommand>
{
    public GenerateChangeUserEmailTokenCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.EmailConfirmationCallbackUrlTemplate)
            .NotNull();
    }
}