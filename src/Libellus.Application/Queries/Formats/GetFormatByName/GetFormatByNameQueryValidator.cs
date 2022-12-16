using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Formats.GetFormatByName;

public sealed class GetFormatByNameQueryValidator : AbstractValidator<GetFormatByNameQuery>
{
    public GetFormatByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}