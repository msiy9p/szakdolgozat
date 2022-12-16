using FluentValidation;

namespace Libellus.Application.Commands.Books.DeleteBook;

public sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}