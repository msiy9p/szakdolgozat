using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Users.GetUserIdByEmail;

// No Authorisation
public sealed record GetUserIdByEmailQuery(string Email) : IQuery<UserId>;