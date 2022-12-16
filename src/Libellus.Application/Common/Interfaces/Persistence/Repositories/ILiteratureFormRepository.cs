using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ILiteratureFormReadOnlyRepository : IReadOnlyRepository<LiteratureForm, LiteratureFormId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(LiteratureFormId id);

    Task<Result<LiteratureForm>> FindByNameAsync(ShortName name, CancellationToken cancellationToken = default);

    Task<Result<ICollection<LiteratureForm>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface ILiteratureFormRepository : ILiteratureFormReadOnlyRepository,
    IRepository<LiteratureForm, LiteratureFormId>
{
}