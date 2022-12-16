using FluentValidation;

namespace Libellus.Application.Commands.Groups.UpdateGroup;

public sealed class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}