using FluentValidation;

namespace Libellus.Application.Commands.WarningTags.DeleteWarningTag;

public sealed class DeleteWarningTagCommandValidator : AbstractValidator<DeleteWarningTagCommand>
{
    public DeleteWarningTagCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}