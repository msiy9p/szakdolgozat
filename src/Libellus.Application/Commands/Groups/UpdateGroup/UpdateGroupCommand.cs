using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Groups.UpdateGroup;

public sealed record UpdateGroupCommand(Group Item) : ICommand;