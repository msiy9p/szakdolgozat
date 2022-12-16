using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Publishers.DeletePublisher;

public sealed record DeletePublisherCommand(Publisher Item) : ICommand;