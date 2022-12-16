using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IBookReadOnlyRepository : IReadOnlyRepository<Book, BookId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<int>> GetBookEditionCountAsync(BookId id, CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(BookId id);

    Task<Result<Book>> GetByIdWithBookEditionsAsync(BookId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<BookCompactVm>> GetCompactVmByIdAsync(BookId id, CancellationToken cancellationToken = default);

    Task<Result<ICollection<BookCompactVm>>> GetAllCompactVmByShelfIdAsync(ShelfId shelfId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<BookCompactVm>>> GetAllCompactVmBySeriesIdAsync(SeriesId seriesId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<BookCompactVm>>> GetAllCompactVmByAuthorIdAsync(AuthorId authorId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Book>>> FindByTitleAsync(Title title, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Book>>>> FindByTitleAsync(Title title, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Book>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Book>>>> SearchAsync(SearchTerm searchTerm, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Book>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Book>>> GetAllByAuthorIdAsync(AuthorId authorId, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Book>>>> GetAllByAuthorIdAsync(AuthorId authorId,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface IBookRepository : IBookReadOnlyRepository, IRepository<Book, BookId>
{
}