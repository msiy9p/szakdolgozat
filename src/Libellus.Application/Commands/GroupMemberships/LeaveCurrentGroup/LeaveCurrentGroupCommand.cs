using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.GroupMemberships.LeaveCurrentGroup;

public sealed record LeaveCurrentGroupCommand : ICommand
{
    public static readonly LeaveCurrentGroupCommand Instance = new();

    private LeaveCurrentGroupCommand()
    {
    }
}