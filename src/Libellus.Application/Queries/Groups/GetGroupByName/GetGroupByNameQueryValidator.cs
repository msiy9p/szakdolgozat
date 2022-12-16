using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Groups.GetGroupByName;

public sealed class SearchGroupsQueryValidator : AbstractValidator<GetGroupByNameQuery>
{
    public SearchGroupsQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}