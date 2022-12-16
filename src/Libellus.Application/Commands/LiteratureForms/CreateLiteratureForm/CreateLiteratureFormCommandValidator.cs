using FluentValidation;

namespace Libellus.Application.Commands.LiteratureForms.CreateLiteratureForm;

public sealed class CreateLiteratureFormCommandValidator : AbstractValidator<CreateLiteratureFormCommand>
{
    public CreateLiteratureFormCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}