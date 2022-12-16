using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Posts.SearchPosts;

public sealed record SearchPostsQuery(SearchTerm SearchTerm, SortOrder SortOrder) : IQuery<ICollection<Post>>;