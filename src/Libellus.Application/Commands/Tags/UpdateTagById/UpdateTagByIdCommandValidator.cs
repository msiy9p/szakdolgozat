using FluentValidation;

namespace Libellus.Application.Commands.Tags.UpdateTagById;

public sealed class UpdateTagByIdCommandValidator : AbstractValidator<UpdateTagByIdCommand>
{
    public UpdateTagByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}