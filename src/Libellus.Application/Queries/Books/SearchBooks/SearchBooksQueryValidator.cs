using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Books.SearchBooks;

public sealed class SearchBooksQueryValidator : AbstractValidator<SearchBooksQuery>
{
    public SearchBooksQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}