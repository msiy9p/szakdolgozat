using FluentValidation;

namespace Libellus.Application.Commands.Labels.CreateLabel;

public sealed class CreateLabelCommandValidator : AbstractValidator<CreateLabelCommand>
{
    public CreateLabelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}