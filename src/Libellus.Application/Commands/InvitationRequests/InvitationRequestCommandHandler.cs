using Libellus.Application.Commands.InvitationRequests.AcceptInvitationRequest;
using Libellus.Application.Commands.InvitationRequests.DeclineInvitationRequest;
using Libellus.Application.Commands.InvitationRequests.SendInvitationRequest;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Application.Commands.InvitationRequests;

public sealed class InvitationRequestCommandHandler :
    ICommandHandler<SendInvitationRequestCommand>,
    ICommandHandler<AcceptInvitationRequestCommand>,
    ICommandHandler<DeclineInvitationRequestCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentGroupService _currentGroupService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IInvitationRequestRepository _invitationRequestRepository;

    public InvitationRequestCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork,
        ICurrentGroupService currentGroupService, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IInvitationRequestRepository InvitationRequestRepository)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _currentGroupService = currentGroupService;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _invitationRequestRepository = InvitationRequestRepository;
    }

    public async Task<Result> Handle(SendInvitationRequestCommand request, CancellationToken cancellationToken)
    {
        var groupId = _currentGroupService.CurrentGroupId;
        if (!groupId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult();
        }

        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var group = await _groupRepository.GetByIdAsync(groupId.Value, cancellationToken);
        if (group.IsError)
        {
            return Result.Error(group.Errors);
        }

        var datetime = _dateTimeProvider.UtcNow;
        var invitation = InvitationRequest.Create(InvitationId.Create(), datetime, datetime, userId.Value,
            groupId.Value,
            InvitationStatus.Pending);
        if (invitation.IsError)
        {
            return Result.Error(invitation.Errors);
        }

        var result = await _invitationRequestRepository.AddIfNotExistsAsync(invitation.Value, cancellationToken);
        if (result.IsError)
        {
            return Result.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(AcceptInvitationRequestCommand request, CancellationToken cancellationToken)
    {
        var groupId = _currentGroupService.CurrentGroupId;
        if (!groupId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult();
        }

        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var invitationRequest =
            await _invitationRequestRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitationRequest.IsError)
        {
            return Result.Error(invitationRequest.Errors);
        }

        invitationRequest.Value.Accept(_dateTimeProvider);

        var updateResult = await _invitationRequestRepository.UpdateAsync(invitationRequest.Value, cancellationToken);
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

    public async Task<Result> Handle(DeclineInvitationRequestCommand request, CancellationToken cancellationToken)
    {
        var groupId = _currentGroupService.CurrentGroupId;
        if (!groupId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult();
        }

        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var invitationRequest =
            await _invitationRequestRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitationRequest.IsError)
        {
            return Result.Error(invitationRequest.Errors);
        }

        invitationRequest.Value.Decline(_dateTimeProvider);

        var updateResult = await _invitationRequestRepository.UpdateAsync(invitationRequest.Value, cancellationToken);
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