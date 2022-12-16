using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.WarningTags.DeleteWarningTag;

public sealed record DeleteWarningTagCommand(WarningTag Item) : ICommand;