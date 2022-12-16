using FluentValidation;

namespace Libellus.Application.Commands.Posts.UpdatePostById;

public sealed class UpdatePostByIdCommandValidator : AbstractValidator<UpdatePostByIdCommand>
{
    public UpdatePostByIdCommandValidator()
    {
        RuleFor(x => x.CommentText)
            .NotNull();
    }
}