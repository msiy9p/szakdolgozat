using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IBookEditionReleasingRepository
{
    Task<Result<ICollection<BookEditionId>>> GetAllIdsReleasingAsync(CancellationToken cancellationToken = default);

    Task<Result<BookEditionReleasingVm>> GetReleasingVmByIdAsync(BookEditionId id,
        CancellationToken cancellationToken = default);
}