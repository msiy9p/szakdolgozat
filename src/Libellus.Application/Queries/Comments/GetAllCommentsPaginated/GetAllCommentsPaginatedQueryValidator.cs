using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Comments.GetAllCommentsPaginated;

public sealed class GetAllCommentsPaginatedQueryValidator : AbstractValidator<GetAllCommentsPaginatedQuery>
{
    public GetAllCommentsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}