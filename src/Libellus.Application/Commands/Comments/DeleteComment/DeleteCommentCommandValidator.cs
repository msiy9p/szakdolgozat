﻿using FluentValidation;

namespace Libellus.Application.Commands.Comments.DeleteComment;

public sealed class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}