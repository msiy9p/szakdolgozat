using FluentValidation;

namespace Libellus.Application.Commands.Authors.UpdateAuthor;

public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}