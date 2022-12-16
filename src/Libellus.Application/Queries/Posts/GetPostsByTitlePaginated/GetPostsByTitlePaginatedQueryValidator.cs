using FluentValidation;

namespace Libellus.Application.Queries.Posts.GetPostsByTitlePaginated;

public sealed class GetPostsByTitlePaginatedQueryValidator : AbstractValidator<GetPostsByTitlePaginatedQuery>
{
    public GetPostsByTitlePaginatedQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}