using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Users.GetAuthenticatorKeyById;

public sealed record GetAuthenticatorKeyByIdQuery(UserId UserId) : IQuery<string>;