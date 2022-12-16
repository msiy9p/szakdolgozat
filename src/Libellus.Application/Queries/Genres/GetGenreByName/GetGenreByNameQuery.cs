using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Genres.GetGenreByName;

public sealed record GetGenreByNameQuery(ShortName Name) : IQuery<Genre>;