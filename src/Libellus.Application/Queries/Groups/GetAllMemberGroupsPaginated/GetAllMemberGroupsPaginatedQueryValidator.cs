using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Groups.GetAllMemberGroupsPaginated;

public sealed class GetAllMemberGroupsPaginatedQueryValidator :
    AbstractValidator<GetAllMemberGroupsPaginatedQuery>
{
    public GetAllMemberGroupsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}