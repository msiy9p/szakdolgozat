using FluentValidation;

namespace Libellus.Application.Commands.Posts.CreatePost;

public sealed class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();

        RuleFor(x => x.CommentText)
            .NotNull();
    }
}