using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Books.GetAllBooksPaginated;

public sealed class GetAllBooksPaginatedQueryValidator : AbstractValidator<GetAllBooksPaginatedQuery>
{
    public GetAllBooksPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}