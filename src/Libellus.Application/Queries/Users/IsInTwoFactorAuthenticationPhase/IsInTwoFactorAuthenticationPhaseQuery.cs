using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Queries.Users.IsInTwoFactorAuthenticationPhase;

// No Authorisation
public sealed record IsInTwoFactorAuthenticationPhaseQuery : IQuery<bool>
{
    public static readonly IsInTwoFactorAuthenticationPhaseQuery Instance = new();

    private IsInTwoFactorAuthenticationPhaseQuery()
    {
    }
}