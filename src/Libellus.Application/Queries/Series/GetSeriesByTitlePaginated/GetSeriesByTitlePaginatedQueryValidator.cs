using FluentValidation;

namespace Libellus.Application.Queries.Series.GetSeriesByTitlePaginated;

public sealed class GetSeriesByTitlePaginatedQueryValidator : AbstractValidator<GetSeriesByTitlePaginatedQuery>
{
    public GetSeriesByTitlePaginatedQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}