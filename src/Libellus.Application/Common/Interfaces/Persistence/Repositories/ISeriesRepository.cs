using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ISeriesReadOnlyRepository : IReadOnlyRepository<Series, SeriesId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<int>> GetBookCountAsync(SeriesId id, CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(SeriesId id);

    Task<Result<Series>> GetByIdWithBooksAsync(SeriesId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<Series>>> GetByIdWithBooksAsync(SeriesId id, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Series>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Series>>> FindByTitleAsync(Title title, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Series>>>> FindByTitleAsync(Title title, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Series>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Series>>>> SearchAsync(SearchTerm searchTerm, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);
}

public interface ISeriesRepository : ISeriesReadOnlyRepository, IRepository<Series, SeriesId>
{
}