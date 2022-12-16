using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.GroupMemberships.GetMemberCountById;

[Authorise]
public sealed record GetMemberCountByIdQuery(GroupId GroupId) : IQuery<int>;