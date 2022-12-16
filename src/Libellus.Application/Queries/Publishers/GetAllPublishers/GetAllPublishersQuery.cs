using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Publishers.GetAllPublishers;

public sealed record GetAllPublishersQuery(SortOrder SortOrder) : IQuery<ICollection<Publisher>>
{
    public static readonly GetAllPublishersQuery DefaultInstance = new(SortOrder.Ascending);
}