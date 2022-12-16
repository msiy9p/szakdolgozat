using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Tags.GetAllTags;

public sealed record GetAllTagsQuery(SortOrder SortOrder) : IQuery<ICollection<Tag>>
{
    public static readonly GetAllTagsQuery DefaultInstance = new(SortOrder.Ascending);
}