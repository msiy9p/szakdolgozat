using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Posts.GetAllPostsPaginated;

public sealed class GetAllPostsPaginatedQueryValidator : AbstractValidator<GetAllPostsPaginatedQuery>
{
    public GetAllPostsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}