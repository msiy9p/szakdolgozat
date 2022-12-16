using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Users.GetEmailById;

public sealed record GetEmailByIdQuery(UserId UserId) : IQuery<string>;