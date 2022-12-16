using FluentValidation;

namespace Libellus.Application.Commands.WarningTags.UpdateWarningTag;

public sealed class UpdateWarningTagCommandValidator : AbstractValidator<UpdateWarningTagCommand>
{
    public UpdateWarningTagCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}