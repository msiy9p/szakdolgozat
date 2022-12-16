using FluentValidation;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Genres.GetGenreByName;

public sealed class GetGenreByNameQueryValidator : AbstractValidator<GetGenreByNameQuery>
{
    public GetGenreByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}