using FluentValidation;

namespace Libellus.Application.Commands.Groups.DeleteGroup;

public sealed class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
{
    public DeleteGroupCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}