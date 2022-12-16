using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Tags.CreateTag;

public sealed record CreateTagCommand(ShortName Name) : ICommand<TagId>;