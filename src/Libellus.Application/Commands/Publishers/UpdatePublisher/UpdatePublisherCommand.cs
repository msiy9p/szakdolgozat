using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Publishers.UpdatePublisher;

public sealed record UpdatePublisherCommand(Publisher Item) : ICommand;