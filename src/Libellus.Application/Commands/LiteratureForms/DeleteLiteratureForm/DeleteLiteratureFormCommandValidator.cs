using FluentValidation;

namespace Libellus.Application.Commands.LiteratureForms.DeleteLiteratureForm;

public sealed class DeleteLiteratureFormCommandValidator : AbstractValidator<DeleteLiteratureFormCommand>
{
    public DeleteLiteratureFormCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}