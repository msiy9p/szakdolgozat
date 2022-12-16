using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Posts.SearchPosts;

public sealed class SearchPostsQueryValidator : AbstractValidator<SearchPostsQuery>
{
    public SearchPostsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}