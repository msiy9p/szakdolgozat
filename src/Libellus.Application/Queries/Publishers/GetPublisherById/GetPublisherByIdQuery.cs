using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Publishers.GetPublisherById;

public sealed record GetPublisherByIdQuery(PublisherId PublisherId) : IQuery<Publisher>;