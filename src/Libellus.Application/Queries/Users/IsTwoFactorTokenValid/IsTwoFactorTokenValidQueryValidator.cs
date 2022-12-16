using FluentValidation;

namespace Libellus.Application.Queries.Users.IsTwoFactorTokenValid;

public sealed class IsTwoFactorTokenValidQueryValidator : AbstractValidator<IsTwoFactorTokenValidQuery>
{
    public IsTwoFactorTokenValidQueryValidator()
    {
        RuleFor(x => x.TwoFactorCode)
            .NotEmpty()
            .Must(IsValid);
    }

    private static bool IsValid(string authCode)
    {
        if (string.IsNullOrWhiteSpace(authCode))
        {
            return false;
        }

        var code = authCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        if (code.Length != 6)
        {
            return false;
        }

        foreach (var number in code)
        {
            if (!char.IsNumber(number))
            {
                return false;
            }
        }

        return true;
    }
}