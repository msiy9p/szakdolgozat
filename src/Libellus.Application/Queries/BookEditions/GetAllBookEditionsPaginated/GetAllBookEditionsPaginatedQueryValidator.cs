using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.BookEditions.GetAllBookEditionsPaginated;

public sealed class GetAllBookEditionsPaginatedQueryValidator : AbstractValidator<GetAllBookEditionsPaginatedQuery>
{
    public GetAllBookEditionsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}