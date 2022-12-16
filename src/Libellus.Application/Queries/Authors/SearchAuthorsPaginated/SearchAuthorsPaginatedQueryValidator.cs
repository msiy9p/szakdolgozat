using FluentValidation;

namespace Libellus.Application.Queries.Authors.SearchAuthorsPaginated;

public sealed class SearchAuthorsPaginatedQueryValidator : AbstractValidator<SearchAuthorsPaginatedQuery>
{
    public SearchAuthorsPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}