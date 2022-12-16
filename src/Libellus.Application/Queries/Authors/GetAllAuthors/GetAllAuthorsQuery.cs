using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Authors.GetAllAuthors;

public sealed record GetAllAuthorsQuery(SortOrder SortOrder) : IQuery<ICollection<Author>>
{
    public static readonly GetAllAuthorsQuery DefaultInstance = new(SortOrder.Ascending);
}