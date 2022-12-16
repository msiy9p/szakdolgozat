using Libellus.Application.Enums;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IInvitationReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result<Invitation>> GetByIdAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result<InvitationVm>> GetVmByIdAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllAsync(InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllToExpireAsync(IDateTimeProvider dateTimeProvider,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllByInviteeIdAsync(UserId inviteeId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllByInviteeIdAsync(UserId inviteeId, InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllByGroupIdAsync(GroupId groupId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Invitation>>> GetAllByGroupIdAsync(GroupId groupId, InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationPicturedVm>>> GetAllPicturedVmByGroupIdAsync(GroupId groupId,
        InvitationStatus status, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<InvitationUserVm>>> GetAllVmByInviteeIdAsync(UserId inviteeId, InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);
}

public interface IInvitationRepository : IInvitationReadOnlyRepository
{
    Task<Result> AddIfNotExistsAsync(Invitation entity, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(Invitation entity, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(InvitationId id, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Invitation entity, CancellationToken cancellationToken = default);
}