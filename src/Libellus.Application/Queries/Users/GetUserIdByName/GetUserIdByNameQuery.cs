using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Users.GetUserIdByName;

// No Authorisation
public sealed record GetUserIdByNameQuery(UserName UserName) : IQuery<UserId>;