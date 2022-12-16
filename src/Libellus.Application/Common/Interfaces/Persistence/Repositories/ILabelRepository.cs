using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ILabelReadOnlyRepository : IReadOnlyRepository<Label, LabelId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<Label>> FindByNameAsync(ShortName name, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Label>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface ILabelRepository : ILabelReadOnlyRepository, IRepository<Label, LabelId>
{
}