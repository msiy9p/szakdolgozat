using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Groups.DeleteGroup;

public sealed record DeleteGroupCommand(Group Item) : ICommand;