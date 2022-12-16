using FluentValidation;

namespace Libellus.Application.Queries.Books.SearchBooksPaginated;

public sealed class SearchBooksPaginatedQueryValidator : AbstractValidator<SearchBooksPaginatedQuery>
{
    public SearchBooksPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}