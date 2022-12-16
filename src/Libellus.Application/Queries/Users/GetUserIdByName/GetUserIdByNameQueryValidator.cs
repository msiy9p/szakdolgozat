using FluentValidation;

namespace Libellus.Application.Queries.Users.GetUserIdByName;

public sealed class GetUserIdByNameQueryValidator : AbstractValidator<GetUserIdByNameQuery>
{
    public GetUserIdByNameQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull();
    }
}