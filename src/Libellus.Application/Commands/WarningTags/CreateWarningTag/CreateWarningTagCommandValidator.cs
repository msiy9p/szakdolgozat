using FluentValidation;

namespace Libellus.Application.Commands.WarningTags.CreateWarningTag;

public sealed class CreateWarningTagCommandValidator : AbstractValidator<CreateWarningTagCommand>
{
    public CreateWarningTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}