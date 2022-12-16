using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Genres.GetAllGenres;

public sealed record GetAllGenresQuery(SortOrder SortOrder) : IQuery<ICollection<Genre>>
{
    public static readonly GetAllGenresQuery DefaultInstance = new(SortOrder.Ascending);
}