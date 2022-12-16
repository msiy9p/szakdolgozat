using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Genres.DeleteGenreById;

public sealed record DeleteGenreByIdCommand(GenreId GenreId) : ICommand;