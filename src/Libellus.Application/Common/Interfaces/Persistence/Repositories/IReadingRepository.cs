using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IReadingReadOnlyRepository : IReadOnlyRepository<Reading, ReadingId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<UserId?>> GetCreatorIdAsync(ReadingId id,
        CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(ReadingId id);

    Task<Result<PaginationDetail<ICollection<Reading>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface IReadingRepository : IReadingReadOnlyRepository, IRepository<Reading, ReadingId>
{
}