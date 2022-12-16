using FluentValidation;

namespace Libellus.Application.Commands.Comments.UpdateCommentById;

public sealed class UpdateCommentByIdCommandValidator : AbstractValidator<UpdateCommentByIdCommand>
{
    public UpdateCommentByIdCommandValidator()
    {
        RuleFor(x => x.CommentText)
            .NotNull();
    }
}