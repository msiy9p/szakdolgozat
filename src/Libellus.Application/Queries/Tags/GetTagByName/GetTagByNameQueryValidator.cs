using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Tags.GetTagByName;

public sealed class GetTagByNameQueryValidator : AbstractValidator<GetTagByNameQuery>
{
    public GetTagByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}