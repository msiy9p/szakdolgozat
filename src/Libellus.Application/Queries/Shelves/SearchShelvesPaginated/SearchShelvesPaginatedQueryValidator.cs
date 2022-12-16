using FluentValidation;

namespace Libellus.Application.Queries.Shelves.SearchShelvesPaginated;

public sealed class SearchShelvesPaginatedQueryValidator : AbstractValidator<SearchShelvesPaginatedQuery>
{
    public SearchShelvesPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}