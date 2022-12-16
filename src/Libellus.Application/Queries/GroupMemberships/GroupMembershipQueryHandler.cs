using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.GroupMemberships.GetGroupMembershipById;
using Libellus.Application.Queries.GroupMemberships.GetMemberCountById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.GroupMemberships;

public sealed class GroupMembershipQueryHandler :
    IQueryHandler<GetGroupMembershipByIdQuery, GroupMembership>,
    IQueryHandler<GetMemberCountByIdQuery, int>
{
    private readonly IGroupMembershipReadOnlyRepository _repository;

    public GroupMembershipQueryHandler(IGroupMembershipReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GroupMembership>> Handle(GetGroupMembershipByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.GroupId, cancellationToken);
    }

    public async Task<Result<int>> Handle(GetMemberCountByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.MemberCountAsync(request.GroupId, cancellationToken);
    }
}