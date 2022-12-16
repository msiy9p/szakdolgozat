using FluentValidation;

namespace Libellus.Application.Commands.WarningTags.UpdateWarningTagById;

public sealed class UpdateWarningTagByIdCommandValidator : AbstractValidator<UpdateWarningTagByIdCommand>
{
    public UpdateWarningTagByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}