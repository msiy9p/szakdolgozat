using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Shelves.GetShelfByIdWithBooksPaginated;

public sealed class
    GetShelfByIdWithBooksPaginatedQueryValidator : AbstractValidator<GetShelfByIdWithBooksPaginatedQuery>
{
    public GetShelfByIdWithBooksPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}