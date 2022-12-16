using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface INoteReadOnlyRepository : IReadOnlyRepository<Note, NoteId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<UserId?>> GetCreatorIdAsync(NoteId id,
        CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(NoteId id);
}

public interface INoteRepository : INoteReadOnlyRepository, IRepository<Note, NoteId>
{
}