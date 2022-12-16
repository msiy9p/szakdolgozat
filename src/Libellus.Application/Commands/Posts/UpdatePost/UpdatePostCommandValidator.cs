using FluentValidation;

namespace Libellus.Application.Commands.Posts.UpdatePost;

public sealed class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}