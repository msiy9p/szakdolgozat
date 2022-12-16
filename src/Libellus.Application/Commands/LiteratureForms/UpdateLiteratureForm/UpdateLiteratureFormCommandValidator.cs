using FluentValidation;

namespace Libellus.Application.Commands.LiteratureForms.UpdateLiteratureForm;

public sealed class UpdateLiteratureFormCommandValidator : AbstractValidator<UpdateLiteratureFormCommand>
{
    public UpdateLiteratureFormCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}