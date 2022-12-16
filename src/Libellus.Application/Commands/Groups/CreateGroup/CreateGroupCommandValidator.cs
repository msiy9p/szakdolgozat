using FluentValidation;

namespace Libellus.Application.Commands.Groups.CreateGroup;

public sealed class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}