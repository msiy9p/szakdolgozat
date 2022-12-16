using Libellus.Application.Commands.Invitations.AcceptInvitation;
using Libellus.Application.Commands.Invitations.DeclineInvitation;
using Libellus.Application.Commands.Invitations.InviteUser;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Application.Commands.Invitations;

public sealed class InvitationCommandHandler :
    ICommandHandler<InviteUserCommand, InvitationId>,
    ICommandHandler<AcceptInvitationCommand>,
    ICommandHandler<DeclineInvitationCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentGroupService _currentGroupService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IInvitationRepository _invitationRepository;

    public InvitationCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork,
        ICurrentGroupService currentGroupService, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IInvitationRepository invitationRepository)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _currentGroupService = currentGroupService;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _invitationRepository = invitationRepository;
    }

    public async Task<Result<InvitationId>> Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        var groupId = _currentGroupService.CurrentGroupId;
        if (!groupId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult<InvitationId>();
        }

        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult<InvitationId>();
        }

        var group = await _groupRepository.GetByIdAsync(groupId.Value, cancellationToken);
        if (group.IsError)
        {
            return Result<InvitationId>.Error(group.Errors);
        }

        var invitationId = group.Value.InviteUser(userId.Value, request.InviteeId, _dateTimeProvider);
        if (invitationId is null)
        {
            return DomainErrors.InvitationErrors.CountNotCreateInvitation.ToErrorResult<InvitationId>();
        }

        var result = await _groupRepository.UpdateAsync(group.Value, cancellationToken);
        if (result.IsError)
        {
            return Result<InvitationId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<InvitationId>.Error(saveResult.Errors);
        }

        return Result<InvitationId>.Success(invitationId.Value);
    }

    public async Task<Result> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitation.IsError)
        {
            return Result.Error(invitation.Errors);
        }

        if (invitation.Value.InvitationStatus != InvitationStatus.Pending
            || invitation.Value.InviteeId != userId)
        {
            return Result.Error(new Error("InvalidInvitation.", "Invalid invitation"));
        }

        invitation.Value.Accept(_dateTimeProvider);

        var updateResult = await _invitationRepository.UpdateAsync(invitation.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return Result.Error(updateResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitation.IsError)
        {
            return Result.Error(invitation.Errors);
        }

        if (invitation.Value.InvitationStatus != InvitationStatus.Pending
            || invitation.Value.InviteeId != userId)
        {
            return Result.Error(new Error("InvalidInvitation.", "Invalid invitation"));
        }

        invitation.Value.Decline(_dateTimeProvider);

        var updateResult = await _invitationRepository.UpdateAsync(invitation.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return Result.Error(updateResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }
}