using Libellus.Application.Enums;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IInvitationRequestReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result<InvitationRequest>> GetByIdAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result<InvitationRequestVm>> GetVmByIdAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationRequest>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationRequest>>> GetAllAsync(InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationRequest>>> GetAllToExpireAsync(IDateTimeProvider dateTimeProvider,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationRequest>>> GetAllByGroupIdAsync(GroupId groupId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationRequest>>> GetAllByGroupIdAsync(GroupId groupId, InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);
}

public interface IInvitationRequestRepository : IInvitationRequestReadOnlyRepository
{
    Task<Result> AddIfNotExistsAsync(InvitationRequest entity, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(InvitationRequest entity, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(InvitationRequest entity, CancellationToken cancellationToken = default);
}