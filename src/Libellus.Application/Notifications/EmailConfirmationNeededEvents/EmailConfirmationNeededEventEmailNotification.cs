using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Notifications.EmailConfirmationNeededEvents;

public sealed class EmailConfirmationNeededEventEmailNotification :
    INotificationHandler<EmailConfirmationNeededEvent>
{
    private readonly IEmailService _emailService;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly ILogger<EmailConfirmationNeededEventEmailNotification> _logger;

    public EmailConfirmationNeededEventEmailNotification(IEmailService emailService,
        IUserReadOnlyRepository userReadOnlyRepository,
        ILogger<EmailConfirmationNeededEventEmailNotification> logger)
    {
        _emailService = emailService;
        _userReadOnlyRepository = userReadOnlyRepository;
        _logger = logger;
    }

    public async Task Handle(EmailConfirmationNeededEvent notification, CancellationToken cancellationToken)
    {
        var userResult = await _userReadOnlyRepository.GetEmailVmByIdAsync(notification.UserId, cancellationToken);
        if (userResult.IsError)
        {
            _logger.LogWarning("Error while getting user email, {UserId}.",
                notification.UserId);

            return;
        }

        _logger.LogWarning("Sending email confirmation email, {UserId}.",
            notification.UserId);

        await _emailService.SendEmailConfirmationAsync(userResult.Value!,
            notification.EmailConfirmationUrl, cancellationToken);
    }
}