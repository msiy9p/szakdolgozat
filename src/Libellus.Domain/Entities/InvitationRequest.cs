using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Events;
using Libellus.Domain.Models;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class InvitationRequest : BaseStampedEntity<InvitationId>
{
    public static readonly Duration ExpireAfterDays = Duration.FromDays(31);

    public GroupId GroupId { get; init; }
    public UserId RequesterId { get; init; }
    public InvitationStatus InvitationStatus { get; private set; }

    internal InvitationRequest(InvitationId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId requesterId, GroupId groupId, InvitationStatus invitationStatus) : base(id, createdOnUtc, modifiedOnUtc)
    {
        RequesterId = requesterId;
        GroupId = groupId;
        InvitationStatus = invitationStatus;
    }

    public static Result<InvitationRequest> Create(InvitationId id, ZonedDateTime createdOnUtc,
        ZonedDateTime modifiedOnUtc, UserId requesterId, GroupId groupId, InvitationStatus invitationStatus)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<InvitationRequest>.Invalid(result.Errors);
        }

        return Result<InvitationRequest>.Success(new InvitationRequest(id, createdOnUtc, modifiedOnUtc, requesterId,
            groupId, invitationStatus));
    }

    public static Result<InvitationRequest> Create(ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId requesterId, GroupId groupId)
    {
        var id = InvitationId.Create();
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<InvitationRequest>.Invalid(result.Errors);
        }

        return Result<InvitationRequest>.Success(new InvitationRequest(id, createdOnUtc, modifiedOnUtc, requesterId,
            groupId, InvitationStatus.Pending));
    }

    public bool Accept(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null || InvitationStatus != InvitationStatus.Pending)
        {
            return false;
        }

        InvitationStatus = InvitationStatus.Accepted;
        UpdateModifiedOnUtc(dateTimeProvider);

        AddDomainEvent(new InvitationAcceptedEvent(dateTimeProvider.UtcNow, Id, RequesterId, GroupId));

        return true;
    }

    public bool Decline(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null || InvitationStatus != InvitationStatus.Pending)
        {
            return false;
        }

        InvitationStatus = InvitationStatus.Declined;
        UpdateModifiedOnUtc(dateTimeProvider);

        return true;
    }

    public bool Expire(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null || InvitationStatus != InvitationStatus.Pending)
        {
            return false;
        }

        InvitationStatus = InvitationStatus.Expired;
        UpdateModifiedOnUtc(dateTimeProvider);

        return true;
    }
}