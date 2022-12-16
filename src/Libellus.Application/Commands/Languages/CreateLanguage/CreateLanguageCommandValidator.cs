using FluentValidation;

namespace Libellus.Application.Commands.Languages.CreateLanguage;

public sealed class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}