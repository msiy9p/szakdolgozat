using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Series.GetSeriesByTitle;

public sealed class SearchSeriesQueryValidator : AbstractValidator<GetSeriesByTitleQuery>
{
    public SearchSeriesQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}