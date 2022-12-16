using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Posts.GetPostsByTitle;

public sealed class SearchPostsQueryValidator : AbstractValidator<GetPostsByTitleQuery>
{
    public SearchPostsQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}