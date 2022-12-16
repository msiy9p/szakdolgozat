using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Events;
using Libellus.Domain.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.DataModels.Contexts.Interceptors;

internal sealed class DomainEventDispatchInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DomainEventDispatchInterceptor> _logger;

    public DomainEventDispatchInterceptor(IPublisher publisher, ILogger<DomainEventDispatchInterceptor> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _publisher = publisher;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null && eventData.Context is not ApplicationContext)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var applicationContext = eventData.Context as ApplicationContext;

        var changed = false;
        if (applicationContext!._domainEventContainer.HasDomainEvents)
        {
            foreach (var domainEvent in applicationContext._domainEventContainer.DomainEvents)
            {
                if (domainEvent is PostUnlockedEvent postUnlockedEvent)
                {
                    _logger.LogInformation("Unlocking post {PostId}", postUnlockedEvent.PostId);

                    await applicationContext.LockedPosts
                        .Where(x => x.PostId == postUnlockedEvent.PostId)
                        .ExecuteDeleteAsync(cancellationToken);

                    changed = true;
                    continue;
                }

                if (domainEvent is PostLockedEvent postLockedEvent)
                {
                    var found = await applicationContext.LockedPosts
                        .AnyAsync(x => x.PostId == postLockedEvent.PostId, cancellationToken);
                    if (!found)
                    {
                        _logger.LogInformation("Locking post {PostId}", postLockedEvent.PostId);

                        var lockedPost = new LockedPost(postLockedEvent.PostId, postLockedEvent.UserId,
                            postLockedEvent.LockReason?.Value, postLockedEvent.DateOccurredOnUtc);

                        await applicationContext.LockedPosts.AddAsync(lockedPost, cancellationToken);
                    }

                    changed = true;
                    continue;
                }

                if (domainEvent is InvitationAcceptedEvent invitationAcceptedEvent)
                {
                    var found = await applicationContext.GroupUserMemberships
                        .Where(x => x.GroupId == invitationAcceptedEvent.GroupId)
                        .AnyAsync(x => x.UserId == invitationAcceptedEvent.UserId, cancellationToken);

                    if (!found)
                    {
                        _logger.LogInformation("Adding user {UserId} into group {GroupId}",
                            invitationAcceptedEvent.UserId, invitationAcceptedEvent.GroupId);

                        var memberId = await applicationContext.GroupRoles
                            .Where(x => x.NameNormalized == "Member".ToNormalizedUpperInvariant())
                            .Select(x => x.GroupRoleId)
                            .FirstAsync(cancellationToken);

                        var dateTime = _dateTimeProvider.UtcNow;

                        var item = new GroupUserMembership(invitationAcceptedEvent.GroupId,
                            invitationAcceptedEvent.UserId, memberId, dateTime, dateTime);

                        await applicationContext.AddAsync(item, cancellationToken);
                    }

                    changed = true;
                    continue;
                }

                await _publisher.Publish(domainEvent, cancellationToken);
            }

            applicationContext._domainEventContainer.ClearEvents();
        }

        if (changed)
        {
            await applicationContext.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}