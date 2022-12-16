using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Publishers.DeletePublisherById;

public sealed record DeletePublisherByIdCommand(PublisherId PublisherId) : ICommand;