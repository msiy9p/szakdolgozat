using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Posts.GetPostsByTitle;

public sealed record GetPostsByTitleQuery(Title Title, SortOrder SortOrder) : IQuery<ICollection<Post>>;