using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.GroupMemberships.GetGroupMembershipById;

// TODO: Authorise method?
[Authorise]
public sealed record GetGroupMembershipByIdQuery(GroupId GroupId) : IQuery<GroupMembership>;