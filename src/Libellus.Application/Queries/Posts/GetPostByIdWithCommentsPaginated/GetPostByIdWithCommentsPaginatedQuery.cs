using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Posts.GetPostByIdWithCommentsPaginated;

public sealed record GetPostByIdWithCommentsPaginatedQuery(PostId PostId, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<Post>>;