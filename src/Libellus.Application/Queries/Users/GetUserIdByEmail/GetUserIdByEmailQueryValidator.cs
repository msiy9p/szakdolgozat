using FluentValidation;

namespace Libellus.Application.Queries.Users.GetUserIdByEmail;

public sealed class GetUserIdByEmailQueryValidator : AbstractValidator<GetUserIdByEmailQuery>
{
    public GetUserIdByEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}