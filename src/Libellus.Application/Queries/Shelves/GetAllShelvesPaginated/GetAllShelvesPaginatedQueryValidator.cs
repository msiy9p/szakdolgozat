using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Shelves.GetAllShelvesPaginated;

public sealed class GetAllShelvesPaginatedQueryValidator : AbstractValidator<GetAllShelvesPaginatedQuery>
{
    public GetAllShelvesPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}