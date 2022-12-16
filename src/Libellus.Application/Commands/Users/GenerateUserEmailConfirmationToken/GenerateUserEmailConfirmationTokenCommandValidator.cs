using FluentValidation;

namespace Libellus.Application.Commands.Users.GenerateUserEmailConfirmationToken;

public sealed class GenerateUserEmailConfirmationTokenCommandValidator : AbstractValidator<GenerateUserEmailConfirmationTokenCommand>
{
    public GenerateUserEmailConfirmationTokenCommandValidator()
    {
        RuleFor(x => x.EmailConfirmationCallbackUrlTemplate)
            .NotNull();
    }
}