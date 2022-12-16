using FluentValidation;

namespace Libellus.Application.Queries.Posts.SearchPostsPaginated;

public sealed class SearchPostsPaginatedQueryValidator : AbstractValidator<SearchPostsPaginatedQuery>
{
    public SearchPostsPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}