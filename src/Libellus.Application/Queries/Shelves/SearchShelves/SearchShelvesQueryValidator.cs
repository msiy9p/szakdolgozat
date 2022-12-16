using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Shelves.SearchShelves;

public sealed class SearchShelvesQueryValidator : AbstractValidator<SearchShelvesQuery>
{
    public SearchShelvesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}