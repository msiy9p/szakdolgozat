using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Tags.UpdateTag;

public sealed record UpdateTagCommand(Tag Item) : ICommand;