using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Genres.GetGenreById;

public sealed record GetGenreByIdQuery(GenreId GenreId) : IQuery<Genre>;