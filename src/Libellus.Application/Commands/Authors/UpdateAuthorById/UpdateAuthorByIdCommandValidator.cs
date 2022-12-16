using FluentValidation;

namespace Libellus.Application.Commands.Authors.UpdateAuthorById;

public sealed class UpdateAuthorByIdCommandValidator :
    AbstractValidator<UpdateAuthorByIdCommand>
{
    public UpdateAuthorByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}