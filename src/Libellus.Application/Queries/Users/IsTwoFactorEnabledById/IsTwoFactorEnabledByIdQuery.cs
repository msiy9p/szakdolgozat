using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Users.IsTwoFactorEnabledById;

public sealed record IsTwoFactorEnabledByIdQuery(UserId UserId) : IQuery<bool>;