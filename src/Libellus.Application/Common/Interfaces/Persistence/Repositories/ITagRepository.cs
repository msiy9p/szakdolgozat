using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ITagReadOnlyRepository : IReadOnlyRepository<Tag, TagId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(TagId id);

    Task<Result<Tag>> FindByNameAsync(ShortName name, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Tag>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<Tag>>> GetAllByBookIdAsync(BookId bookId, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface ITagRepository : ITagReadOnlyRepository, IRepository<Tag, TagId>
{
}