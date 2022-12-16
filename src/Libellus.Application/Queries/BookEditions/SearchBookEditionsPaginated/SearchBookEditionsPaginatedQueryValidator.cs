using FluentValidation;

namespace Libellus.Application.Queries.BookEditions.SearchBookEditionsPaginated;

public sealed class SearchBookEditionsPaginatedQueryValidator : AbstractValidator<SearchBookEditionsPaginatedQuery>
{
    public SearchBookEditionsPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}