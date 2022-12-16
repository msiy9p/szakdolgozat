using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Groups.SearchGroups;

public sealed class SearchGroupsQueryValidator : AbstractValidator<SearchGroupsQuery>
{
    public SearchGroupsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotNull();
    }
}