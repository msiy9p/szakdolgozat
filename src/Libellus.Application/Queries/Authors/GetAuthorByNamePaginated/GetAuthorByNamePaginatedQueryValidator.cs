using FluentValidation;

namespace Libellus.Application.Queries.Authors.GetAuthorByNamePaginated;

public sealed class GetAuthorByNamePaginatedQueryValidator : AbstractValidator<GetAuthorByNamePaginatedQuery>
{
    public GetAuthorByNamePaginatedQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}