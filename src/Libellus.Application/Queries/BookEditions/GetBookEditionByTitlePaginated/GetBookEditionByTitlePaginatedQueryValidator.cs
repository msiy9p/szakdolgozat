using FluentValidation;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByTitlePaginated;

public sealed class GetBookEditionByTitlePaginatedQueryValidator :
    AbstractValidator<GetBookEditionByTitlePaginatedQuery>
{
    public GetBookEditionByTitlePaginatedQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}