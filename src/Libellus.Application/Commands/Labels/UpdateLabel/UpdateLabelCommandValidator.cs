using FluentValidation;

namespace Libellus.Application.Commands.Labels.UpdateLabel;

public sealed class UpdateLabelCommandValidator : AbstractValidator<UpdateLabelCommand>
{
    public UpdateLabelCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}