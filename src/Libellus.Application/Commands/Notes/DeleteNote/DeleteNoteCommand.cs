using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Notes.DeleteNote;

public sealed record DeleteNoteCommand(Note Item) : ICommand;