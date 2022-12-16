using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Formats.GetAllFormats;

public sealed record GetAllFormatsQuery(SortOrder SortOrder) : IQuery<ICollection<Format>>
{
    public static readonly GetAllFormatsQuery DefaultInstance = new(SortOrder.Ascending);
}