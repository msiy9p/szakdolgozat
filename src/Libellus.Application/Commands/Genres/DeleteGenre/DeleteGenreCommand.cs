using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Genres.DeleteGenre;

public sealed record DeleteGenreCommand(Genre Item) : ICommand;