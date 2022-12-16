using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IBookEditionReadOnlyRepository : IReadOnlyRepository<BookEdition, BookEditionId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<int>> GetReadingCountAsync(BookEditionId id, CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(BookEditionId id);

    Task<Result<BookEdition>> GetByIdWithReadingsAsync(BookEditionId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<BookEditionCompactVm>> GetCompactVmByIdAsync(BookEditionId id,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<BookEdition>>> FindByIsbnAsync(Isbn isbn, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<BookEdition>>>> FindByIsbnAsync(Isbn isbn, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<BookEdition>>> FindByTitleAsync(Title title, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<BookEdition>>>> FindByTitleAsync(Title title, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<BookEdition>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<BookEdition>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<BookEdition>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);
}

public interface IBookEditionRepository : IBookEditionReadOnlyRepository, IRepository<BookEdition, BookEditionId>
{
}