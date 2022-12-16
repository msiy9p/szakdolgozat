using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Publishers.CreatePublisher;

public sealed record CreatePublisherCommand(ShortName Name) : ICommand<PublisherId>;