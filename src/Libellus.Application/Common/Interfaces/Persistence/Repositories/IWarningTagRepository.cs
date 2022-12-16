using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IWarningTagReadOnlyRepository : IReadOnlyRepository<WarningTag, WarningTagId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(WarningTagId id);

    Task<Result<WarningTag>> FindByNameAsync(ShortName name, CancellationToken cancellationToken = default);

    Task<Result<ICollection<WarningTag>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<WarningTag>>> GetAllByBookIdAsync(BookId bookId, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface IWarningTagRepository : IWarningTagReadOnlyRepository, IRepository<WarningTag, WarningTagId>
{
}