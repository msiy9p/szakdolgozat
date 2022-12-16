using FluentValidation;

namespace Libellus.Application.Commands.BookEditions.DeleteBookEdition;

public sealed class DeleteBookEditionCommandValidator : AbstractValidator<DeleteBookEditionCommand>
{
    public DeleteBookEditionCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}