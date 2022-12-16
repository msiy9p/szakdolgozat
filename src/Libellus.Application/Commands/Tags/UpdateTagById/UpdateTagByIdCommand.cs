using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Tags.UpdateTagById;

public sealed record UpdateTagByIdCommand(TagId TagId, ShortName Name) : ICommand;