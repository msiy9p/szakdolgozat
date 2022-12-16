using FluentValidation;

namespace Libellus.Application.Commands.Tags.DeleteTag;

public sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}