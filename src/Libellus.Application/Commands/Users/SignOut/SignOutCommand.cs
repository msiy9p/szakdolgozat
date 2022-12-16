using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;

namespace Libellus.Application.Commands.Users.SignOut;

[Authorise]
public sealed record SignOutCommand : ICommand
{
    public static readonly SignOutCommand Instance = new();

    private SignOutCommand()
    {
    }
}