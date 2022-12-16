using FluentValidation;

namespace Libellus.Application.Commands.Languages.UpdateLanguage;

public sealed class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}