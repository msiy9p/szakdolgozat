using FluentValidation;

namespace Libellus.Application.Commands.Comments.CreateComment;

public sealed class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.CommentText)
            .NotNull();
    }
}