using FluentValidation;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Posts.GetPostByIdWithCommentsPaginated;

public sealed class
    GetPostByIdWithCommentsPaginatedQueryValidator : AbstractValidator<GetPostByIdWithCommentsPaginatedQuery>
{
    public GetPostByIdWithCommentsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PaginationInfo.DefaultPageNumber);
    }
}