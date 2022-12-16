using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIsbnPaginated;

public sealed class GetBookEditionByIsbnPaginatedQueryValidator : AbstractValidator<GetBookEditionByIsbnPaginatedQuery>
{
    public GetBookEditionByIsbnPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}