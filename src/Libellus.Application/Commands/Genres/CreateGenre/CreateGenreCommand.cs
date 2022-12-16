using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Genres.CreateGenre;

public sealed record CreateGenreCommand(ShortName Name, bool IsFiction) : ICommand<GenreId>;