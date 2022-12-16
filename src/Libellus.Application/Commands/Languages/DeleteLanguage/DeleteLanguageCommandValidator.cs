using FluentValidation;

namespace Libellus.Application.Commands.Languages.DeleteLanguage;

public sealed class DeleteLanguageCommandValidator : AbstractValidator<DeleteLanguageCommand>
{
    public DeleteLanguageCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}