using FluentValidation;

namespace Libellus.Application.Commands.Shelves.DeleteShelf;

public sealed class DeleteShelfCommandValidator : AbstractValidator<DeleteShelfCommand>
{
    public DeleteShelfCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}