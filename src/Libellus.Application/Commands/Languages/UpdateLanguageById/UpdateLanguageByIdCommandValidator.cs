using FluentValidation;

namespace Libellus.Application.Commands.Languages.UpdateLanguageById;

public sealed class UpdateLanguageByIdCommandValidator : AbstractValidator<UpdateLanguageByIdCommand>
{
    public UpdateLanguageByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}