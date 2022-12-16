using FluentValidation;

namespace Libellus.Application.Commands.Authors.DeleteAuthor;

public sealed class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}