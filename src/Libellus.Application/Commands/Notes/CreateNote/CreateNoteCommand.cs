using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Notes.CreateNote;

public sealed record CreateNoteCommand(ReadingId ReadingId, CommentText CommentText) : ICommand<NoteId>;