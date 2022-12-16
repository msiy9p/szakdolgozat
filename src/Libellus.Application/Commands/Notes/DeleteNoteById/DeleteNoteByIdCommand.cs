using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Notes.DeleteNoteById;

public sealed record DeleteNoteByIdCommand(NoteId NoteId) : ICommand;