using FluentValidation;

namespace Libellus.Application.Commands.Posts.LockPostById;

public sealed class LockPostByIdCommandValidator : AbstractValidator<LockPostByIdCommand>
{
    public LockPostByIdCommandValidator()
    {
        RuleFor(x => x.CommentText)
            .NotNull();
    }
}