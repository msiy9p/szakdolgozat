using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Groups.GetAllGroupsPaginated;

public sealed class GetAllGroupsPaginatedQueryValidator : AbstractValidator<GetAllGroupsPaginatedQuery>
{
    public GetAllGroupsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}