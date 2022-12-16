using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Notes.GetNoteById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Notes;

public sealed class NoteQueryHandler :
    IQueryHandler<GetNoteByIdQuery, Note>
{
    private readonly INoteReadOnlyRepository _repository;

    public NoteQueryHandler(INoteReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Note>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.NoteId, cancellationToken);
    }
}