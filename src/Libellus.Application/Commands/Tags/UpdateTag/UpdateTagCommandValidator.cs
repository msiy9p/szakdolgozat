using FluentValidation;

namespace Libellus.Application.Commands.Tags.UpdateTag;

public sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}