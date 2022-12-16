using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetGroupById;

[Authorise]
public sealed record GetGroupByIdQuery(GroupId GroupId) : IQuery<Group>;