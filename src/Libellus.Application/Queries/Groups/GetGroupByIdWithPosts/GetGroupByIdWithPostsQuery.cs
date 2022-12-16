using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetGroupByIdWithPosts;

[Authorise]
public sealed record GetGroupByIdWithPostsQuery(GroupId GroupId, SortOrder SortOrder) : IQuery<Group>;