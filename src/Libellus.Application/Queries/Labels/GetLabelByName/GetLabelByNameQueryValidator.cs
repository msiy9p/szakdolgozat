using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Labels.GetLabelByName;

public sealed class GetLabelByNameQueryValidator : AbstractValidator<GetLabelByNameQuery>
{
    public GetLabelByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}