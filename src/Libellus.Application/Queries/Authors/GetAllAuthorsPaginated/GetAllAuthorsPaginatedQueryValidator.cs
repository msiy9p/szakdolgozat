using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Authors.GetAllAuthorsPaginated;

public sealed class GetAllAuthorsPaginatedQueryValidator : AbstractValidator<GetAllAuthorsPaginatedQuery>
{
    public GetAllAuthorsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}