using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Notes.GetNoteById;

public sealed record GetNoteByIdQuery(NoteId NoteId) : IQuery<Note>;