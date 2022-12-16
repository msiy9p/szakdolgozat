using FluentValidation;

namespace Libellus.Application.Queries.Groups.GetGroupByNamePaginated;

public sealed class GetGroupByNamePaginatedQueryValidator : AbstractValidator<GetGroupByNamePaginatedQuery>
{
    public GetGroupByNamePaginatedQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}