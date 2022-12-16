using FluentValidation;

namespace Libellus.Application.Commands.BookEditions.CreateBookEdition;

public sealed class CreateBookEditionCommandValidator : AbstractValidator<CreateBookEditionCommand>
{
    public CreateBookEditionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}