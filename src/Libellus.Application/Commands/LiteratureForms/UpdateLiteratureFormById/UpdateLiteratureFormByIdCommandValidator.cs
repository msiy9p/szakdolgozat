using FluentValidation;

namespace Libellus.Application.Commands.LiteratureForms.UpdateLiteratureFormById;

public sealed class UpdateLiteratureFormByIdCommandValidator : AbstractValidator<UpdateLiteratureFormByIdCommand>
{
    public UpdateLiteratureFormByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}