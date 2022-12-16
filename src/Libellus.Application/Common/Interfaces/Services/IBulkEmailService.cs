using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Services;

public interface IBulkEmailService
{
    Task<IReadOnlyList<Result>> SendAsync(IReadOnlyList<IEmailData> emailData,
        CancellationToken cancellationToken = default);

    Task<Result> SendAsync(BookEditionReleasingVm bookEditionReleasing, CancellationToken cancellationToken = default);
}