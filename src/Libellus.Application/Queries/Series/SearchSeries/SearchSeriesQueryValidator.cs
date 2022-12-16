using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Series.SearchSeries;

public sealed class SearchSeriesQueryValidator : AbstractValidator<SearchSeriesQuery>
{
    public SearchSeriesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}