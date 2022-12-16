using FluentValidation;

namespace Libellus.Application.Queries.Groups.SearchGroupsPaginated;

public sealed class SearchGroupsPaginatedQueryValidator : AbstractValidator<SearchGroupsPaginatedQuery>
{
    public SearchGroupsPaginatedQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}