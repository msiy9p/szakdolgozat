using FluentValidation;

namespace Libellus.Application.Commands.Posts.DeletePost;

public sealed class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
{
    public DeletePostCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}