using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IGroupMembershipReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(GroupId id, CancellationToken cancellationToken = default);

    Task<Result<int>> MemberCountAsync(GroupId id, CancellationToken cancellationToken = default);

    Task<Result<GroupMembership>> GetByIdAsync(GroupId id, CancellationToken cancellationToken = default);
}

public interface IGroupMembershipRepository : IGroupMembershipReadOnlyRepository
{
    Task<Result> UpdateAsync(GroupMembership entity, CancellationToken cancellationToken = default);
}