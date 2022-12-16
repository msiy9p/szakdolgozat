using FluentValidation;

namespace Libellus.Application.Queries.Books.GetBookByTitlePaginated;

public sealed class GetBookByTitlePaginatedQueryValidator : AbstractValidator<GetBookByTitlePaginatedQuery>
{
    public GetBookByTitlePaginatedQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}