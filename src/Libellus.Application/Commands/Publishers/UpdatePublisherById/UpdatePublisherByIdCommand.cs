using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Publishers.UpdatePublisherById;

public sealed record UpdatePublisherByIdCommand(PublisherId PublisherId, ShortName Name) : ICommand;