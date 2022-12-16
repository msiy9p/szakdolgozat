using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.WarningTags.UpdateWarningTag;

public sealed record UpdateWarningTagCommand(WarningTag Item) : ICommand;