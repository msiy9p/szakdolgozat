using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Posts.GetAllPosts;

public sealed record GetAllPostsQuery(SortOrder SortOrder) : IQuery<ICollection<Post>>
{
    public static readonly GetAllPostsQuery DefaultInstance = new(SortOrder.Ascending);
}