using FluentValidation;

namespace Libellus.Application.Queries.Authors.SearchAuthors;

public sealed class SearchAuthorsQueryValidator : AbstractValidator<SearchAuthorsQuery>
{
    public SearchAuthorsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}