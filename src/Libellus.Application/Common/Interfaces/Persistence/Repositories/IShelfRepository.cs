using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IShelfReadOnlyRepository : IReadOnlyRepository<Shelf, ShelfId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<int>> GetBookCountAsync(ShelfId id, CancellationToken cancellationToken = default);

    Task<Result<UserId?>> GetCreatorIdAsync(ShelfId id, CancellationToken cancellationToken = default);

    Task<Result<Shelf>> GetByIdWithBooksAsync(ShelfId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<Shelf>>> GetByIdWithBooksAsync(ShelfId id, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Shelf>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<Shelf>> FindByNameAsync(Name name, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Shelf>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Shelf>>>> SearchAsync(SearchTerm searchTerm, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Shelf>>> GetShelvesByBookIdAsync(BookId bookId, bool containing,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);
}

public interface IShelfRepository : IShelfReadOnlyRepository, IRepository<Shelf, ShelfId>
{
}