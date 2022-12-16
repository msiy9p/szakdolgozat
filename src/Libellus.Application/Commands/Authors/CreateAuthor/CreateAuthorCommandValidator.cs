using FluentValidation;

namespace Libellus.Application.Commands.Authors.CreateAuthor;

public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}