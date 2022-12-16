using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Series.GetSeriesByIdWithBooksPaginated;

public sealed class GetSeriesByIdWithBooksPaginatedQueryValidator :
    AbstractValidator<GetSeriesByIdWithBooksPaginatedQuery>
{
    public GetSeriesByIdWithBooksPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}