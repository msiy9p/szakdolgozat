using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Tags.DeleteTag;

public sealed record DeleteTagCommand(Tag Item) : ICommand;