using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.WarningTags.GetAllWarningTags;

public sealed record GetAllWarningTagsQuery(SortOrder SortOrder) : IQuery<ICollection<WarningTag>>
{
    public static readonly GetAllWarningTagsQuery DefaultInstance = new(SortOrder.Ascending);
}