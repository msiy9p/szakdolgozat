using FluentValidation;

namespace Libellus.Application.Commands.Books.UpdateBook;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}