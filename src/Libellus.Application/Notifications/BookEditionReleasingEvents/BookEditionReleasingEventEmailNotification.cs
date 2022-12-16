using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Notifications.BookEditionReleasingEvents;

public sealed class BookEditionReleasingEventEmailNotification : INotificationHandler<BookEditionReleasingEvent>
{
    private readonly IBulkEmailService _bulkEmailService;
    private readonly IBookEditionReleasingRepository _bookEditionReleasingRepository;
    private readonly ILogger<BookEditionReleasingEventEmailNotification> _logger;

    public BookEditionReleasingEventEmailNotification(IBulkEmailService bulkEmailService,
        IBookEditionReleasingRepository bookEditionReleasingRepository,
        ILogger<BookEditionReleasingEventEmailNotification> logger)
    {
        _bulkEmailService = bulkEmailService;
        _bookEditionReleasingRepository = bookEditionReleasingRepository;
        _logger = logger;
    }

    public async Task Handle(BookEditionReleasingEvent notification, CancellationToken cancellationToken)
    {
        var found = await _bookEditionReleasingRepository.GetReleasingVmByIdAsync(notification.BookEditionId,
            cancellationToken);
        if (found.IsError)
        {
            _logger.LogWarning("Error while getting releasing BookEdition, {BookEditionId}.",
                notification.BookEditionId);

            return;
        }

        _logger.LogInformation("Sending releasing BookEdition notification, {BookEditionId}.",
            notification.BookEditionId);

        await _bulkEmailService.SendAsync(found.Value!, cancellationToken);
    }
}