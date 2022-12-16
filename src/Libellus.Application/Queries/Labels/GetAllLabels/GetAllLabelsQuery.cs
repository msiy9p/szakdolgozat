using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Labels.GetAllLabels;

public sealed record GetAllLabelsQuery(SortOrder SortOrder) : IQuery<ICollection<Label>>
{
    public static readonly GetAllLabelsQuery DefaultInstance = new(SortOrder.Ascending);
}