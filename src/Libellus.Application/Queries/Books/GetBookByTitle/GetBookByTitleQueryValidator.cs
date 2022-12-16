using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Books.GetBookByTitle;

public sealed class SearchBooksQueryValidator : AbstractValidator<GetBookByTitleQuery>
{
    public SearchBooksQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}