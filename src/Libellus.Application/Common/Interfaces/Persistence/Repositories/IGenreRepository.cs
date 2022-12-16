using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IGenreReadOnlyRepository : IReadOnlyRepository<Genre, GenreId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(GenreId id);

    Task<Result<Genre>> FindByNameAsync(ShortName name, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Genre>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<Genre>>> GetAllByFictionAsync(bool isFiction = true,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Genre>>> GetAllByBookIdAsync(BookId bookId, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface IGenreRepository : IGenreReadOnlyRepository, IRepository<Genre, GenreId>
{
}