using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Groups.DeleteGroupById;

public sealed record DeleteGroupByIdCommand(GroupId GroupId) : ICommand;