using FluentValidation;

namespace Libellus.Application.Queries.Series.SearchSeriesPaginated;

public sealed class SearchSeriesPaginatedQueryValidator : AbstractValidator<SearchSeriesPaginatedQuery>
{
    public SearchSeriesPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}