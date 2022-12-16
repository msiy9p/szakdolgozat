using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IAuthorReadOnlyRepository : IReadOnlyRepository<Author, AuthorId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(AuthorId id);

    Task<Result<ICollection<Author>>> FindByNameAsync(Name name, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Author>>>> FindByNameAsync(Name name, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Author>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Author>>>> SearchAsync(SearchTerm searchTerm, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Author>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Author>>> GetAllByBookIdAsync(BookId bookId, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface IAuthorRepository : IAuthorReadOnlyRepository, IRepository<Author, AuthorId>
{
}