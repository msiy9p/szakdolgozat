using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Shelves.GetShelfByName;

public sealed class GetShelfByNameQueryValidator : AbstractValidator<GetShelfByNameQuery>
{
    public GetShelfByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}