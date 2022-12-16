using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Users.IsTwoFactorTokenValid;

public sealed record IsTwoFactorTokenValidQuery(UserId UserId, string TwoFactorCode) : IQuery<bool>;