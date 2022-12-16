using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByTitle;

public sealed class SearchBookEditionsQueryValidator : AbstractValidator<GetBookEditionByTitleQuery>
{
    public SearchBookEditionsQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}