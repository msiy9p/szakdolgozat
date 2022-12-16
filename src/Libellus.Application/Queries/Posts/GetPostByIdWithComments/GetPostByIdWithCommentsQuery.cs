using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Posts.GetPostByIdWithComments;

public sealed record GetPostByIdWithCommentsQuery(PostId PostId, SortOrder SortOrder) : IQuery<Post>;