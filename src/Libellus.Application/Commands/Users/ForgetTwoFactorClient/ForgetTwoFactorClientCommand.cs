using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;

namespace Libellus.Application.Commands.Users.ForgetTwoFactorClient;

[Authorise]
public sealed record ForgetTwoFactorClientCommand : ICommand
{
    public static readonly ForgetTwoFactorClientCommand Instance = new();

    private ForgetTwoFactorClientCommand()
    {
    }
}