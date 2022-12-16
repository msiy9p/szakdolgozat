using FluentValidation;

namespace Libellus.Application.Commands.Books.UpdateBookById;

public sealed class UpdateBookByIdCommandValidator : AbstractValidator<UpdateBookByIdCommand>
{
    public UpdateBookByIdCommandValidator()
    {
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