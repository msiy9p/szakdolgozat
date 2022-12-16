using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Languages.GetLanguageByName;

public sealed class GetLanguageByNameQueryValidator : AbstractValidator<GetLanguageByNameQuery>
{
    public GetLanguageByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}