using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Publishers.GetPublisherByName;

public sealed class GetPublisherByNameQueryValidator : AbstractValidator<GetPublisherByNameQuery>
{
    public GetPublisherByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}