using FluentValidation;

namespace Libellus.Application.Commands.Labels.DeleteLabel;

public sealed class DeleteLabelCommandValidator : AbstractValidator<DeleteLabelCommand>
{
    public DeleteLabelCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}