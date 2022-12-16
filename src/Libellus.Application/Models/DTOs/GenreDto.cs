using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public sealed class GenreDto
{
    public ShortName ShortName { get; init; }
    public bool IsFiction { get; init; }
    public GenreId? GenreId { get; init; }

    public GenreDto(ShortName shortName, bool isFiction, GenreId? genreId)
    {
        ShortName = shortName;
        IsFiction = isFiction;
        GenreId = genreId;
    }
}