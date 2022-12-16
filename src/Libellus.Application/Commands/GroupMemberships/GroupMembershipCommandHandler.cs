using Libellus.Application.Commands.GroupMemberships.DemoteModerator;
using Libellus.Application.Commands.GroupMemberships.DemoteOwner;
using Libellus.Application.Commands.GroupMemberships.KickOutMember;
using Libellus.Application.Commands.GroupMemberships.LeaveCurrentGroup;
using Libellus.Application.Commands.GroupMemberships.PromoteModerator;
using Libellus.Application.Commands.GroupMemberships.PromoteUser;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Application.Commands.GroupMemberships;

public sealed class GroupMembershipCommandHandler :
    ICommandHandler<DemoteModeratorCommand>,
    ICommandHandler<DemoteOwnerCommand>,
    ICommandHandler<KickOutMemberCommand>,
    ICommandHandler<PromoteModeratorCommand>,
    ICommandHandler<PromoteUserCommand>,
    ICommandHandler<LeaveCurrentGroupCommand>
{
    private readonly IGroupMembershipRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentGroupService _currentGroupService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public GroupMembershipCommandHandler(IGroupMembershipRepository repository,
        IDateTimeProvider dateTimeProvider,
        ICurrentGroupService currentGroupService, IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _currentGroupService = currentGroupService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    private async Task<Result<GroupMembership>> GetMemberShipAsync(CancellationToken cancellationToken)
    {
        var groupId = _currentGroupService.CurrentGroupId;
        if (!groupId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult<GroupMembership>();
        }

        return await _repository.GetByIdAsync(groupId.Value, cancellationToken);
    }

    public async Task<Result> Handle(DemoteModeratorCommand request, CancellationToken cancellationToken)
    {
        var membership = await GetMemberShipAsync(cancellationToken);
        if (membership.IsError)
        {
            return membership;
        }

        membership.Value.Update(request.UserId, GroupRole.Member, _dateTimeProvider);

        var result = await _repository.UpdateAsync(membership.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DemoteOwnerCommand request, CancellationToken cancellationToken)
    {
        var membership = await GetMemberShipAsync(cancellationToken);
        if (membership.IsError)
        {
            return membership;
        }

        membership.Value.Update(request.UserId, GroupRole.Moderator, _dateTimeProvider);

        var result = await _repository.UpdateAsync(membership.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(KickOutMemberCommand request, CancellationToken cancellationToken)
    {
        var membership = await GetMemberShipAsync(cancellationToken);
        if (membership.IsError)
        {
            return membership;
        }

        membership.Value.Remove(request.UserId);

        var result = await _repository.UpdateAsync(membership.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(PromoteModeratorCommand request, CancellationToken cancellationToken)
    {
        var membership = await GetMemberShipAsync(cancellationToken);
        if (membership.IsError)
        {
            return membership;
        }

        membership.Value.Update(request.UserId, GroupRole.Owner, _dateTimeProvider);

        var result = await _repository.UpdateAsync(membership.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(PromoteUserCommand request, CancellationToken cancellationToken)
    {
        var membership = await GetMemberShipAsync(cancellationToken);
        if (membership.IsError)
        {
            return membership;
        }

        membership.Value.Update(request.UserId, GroupRole.Moderator, _dateTimeProvider);

        var result = await _repository.UpdateAsync(membership.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(LeaveCurrentGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var membership = await GetMemberShipAsync(cancellationToken);
        if (membership.IsError)
        {
            return membership;
        }

        membership.Value.Remove(userId.Value);

        var result = await _repository.UpdateAsync(membership.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}