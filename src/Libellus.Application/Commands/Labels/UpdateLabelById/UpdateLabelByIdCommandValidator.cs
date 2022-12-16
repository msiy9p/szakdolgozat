using FluentValidation;

namespace Libellus.Application.Commands.Labels.UpdateLabelById;

public sealed class UpdateLabelByIdCommandValidator : AbstractValidator<UpdateLabelByIdCommand>
{
    public UpdateLabelByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}