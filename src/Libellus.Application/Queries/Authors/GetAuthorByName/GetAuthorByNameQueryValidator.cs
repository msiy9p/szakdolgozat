using FluentValidation;

namespace Libellus.Application.Queries.Authors.GetAuthorByName;

public sealed class SearchAuthorsQueryValidator : AbstractValidator<GetAuthorByNameQuery>
{
    public SearchAuthorsQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}