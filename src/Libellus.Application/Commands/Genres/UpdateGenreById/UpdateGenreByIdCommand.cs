using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Genres.UpdateGenreById;

public sealed record UpdateGenreByIdCommand(GenreId GenreId, ShortName Name) : ICommand;