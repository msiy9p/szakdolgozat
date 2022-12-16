using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Notifications.UserInvitedEvents;

public sealed class UserInvitedEventEmailNotification : INotificationHandler<UserInvitedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IInvitationReadOnlyRepository _invitationRepository;
    private readonly ILogger<UserInvitedEventEmailNotification> _logger;

    public UserInvitedEventEmailNotification(IEmailService emailService,
        IInvitationReadOnlyRepository invitationRepository,
        ILogger<UserInvitedEventEmailNotification> logger)
    {
        _emailService = emailService;
        _invitationRepository = invitationRepository;
        _logger = logger;
    }

    public async Task Handle(UserInvitedEvent notification, CancellationToken cancellationToken)
    {
        var invitationResult = await _invitationRepository.GetVmByIdAsync(notification.InvitationId, cancellationToken);
        if (invitationResult.IsError)
        {
            _logger.LogWarning("Error while getting user invitation, {InvitationId}.",
                notification.InvitationId);

            return;
        }

        _logger.LogInformation("Sending user invitation, {InvitationId}.",
            notification.InvitationId);

        await _emailService.SendInvitationAsync(invitationResult.Value!, cancellationToken);
    }
}