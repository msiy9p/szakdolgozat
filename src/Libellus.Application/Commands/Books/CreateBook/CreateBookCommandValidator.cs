using FluentValidation;

namespace Libellus.Application.Commands.Books.CreateBook;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();

        RuleFor(x => x.Authors)
            .NotNull();

        RuleFor(x => x.Genres)
            .NotNull();

        RuleFor(x => x.Tags)
            .NotNull();

        RuleFor(x => x.WarningTags)
            .NotNull();
    }
}