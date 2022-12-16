using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.LiteratureForms.GetLiteratureFormByName;

public sealed class GetLiteratureFormByNameQueryValidator : AbstractValidator<GetLiteratureFormByNameQuery>
{
    public GetLiteratureFormByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}