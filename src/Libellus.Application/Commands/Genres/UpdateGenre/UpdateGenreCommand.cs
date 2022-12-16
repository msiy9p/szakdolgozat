using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Genres.UpdateGenre;

public sealed record UpdateGenreCommand(Genre Item) : ICommand;