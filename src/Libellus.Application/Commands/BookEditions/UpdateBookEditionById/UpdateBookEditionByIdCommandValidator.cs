using FluentValidation;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEditionById;

public sealed class UpdateBookEditionByIdCommandValidator : AbstractValidator<UpdateBookEditionByIdCommand>
{
    public UpdateBookEditionByIdCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}