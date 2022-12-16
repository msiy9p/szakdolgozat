using FluentValidation;

namespace Libellus.Application.Commands.Comments.UpdateComment;

public sealed class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}