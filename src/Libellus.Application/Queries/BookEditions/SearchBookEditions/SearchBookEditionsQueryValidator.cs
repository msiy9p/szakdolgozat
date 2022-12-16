using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.BookEditions.SearchBookEditions;

public sealed class SearchBookEditionsQueryValidator : AbstractValidator<SearchBookEditionsQuery>
{
    public SearchBookEditionsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}