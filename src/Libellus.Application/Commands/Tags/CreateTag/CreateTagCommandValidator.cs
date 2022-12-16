using FluentValidation;

namespace Libellus.Application.Commands.Tags.CreateTag;

public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}