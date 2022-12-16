using FluentValidation;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEdition;

public sealed class UpdateBookEditionCommandValidator : AbstractValidator<UpdateBookEditionCommand>
{
    public UpdateBookEditionCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}