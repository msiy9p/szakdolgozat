using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Notes.UpdateNote;

public sealed record UpdateNoteCommand(Note Item) : ICommand;