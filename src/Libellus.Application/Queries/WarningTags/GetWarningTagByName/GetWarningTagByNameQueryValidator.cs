using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.WarningTags.GetWarningTagByName;

public sealed class GetWarningTagByNameQueryValidator : AbstractValidator<GetWarningTagByNameQuery>
{
    public GetWarningTagByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}