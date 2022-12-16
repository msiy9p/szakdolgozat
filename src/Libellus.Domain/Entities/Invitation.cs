using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Events;
using Libellus.Domain.Models;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Invitation : BaseStampedEntity<InvitationId>
{
    public static readonly Duration ExpireAfterDays = Duration.FromDays(31);

    public GroupId GroupId { get; init; }
    public UserId InviterId { get; init; }
    public UserId InviteeId { get; init; }
    public InvitationStatus InvitationStatus { get; private set; }

    internal Invitation(InvitationId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId inviterId,
        GroupId groupId, UserId inviteeId, InvitationStatus invitationStatus) : base(id, createdOnUtc, modifiedOnUtc)
    {
        InviterId = inviterId;
        GroupId = groupId;
        InviteeId = inviteeId;
        InvitationStatus = invitationStatus;
    }

    public static Result<Invitation> Create(InvitationId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId inviterId, GroupId groupId, UserId inviteeId, InvitationStatus invitationStatus)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Invitation>.Invalid(result.Errors);
        }

        return Result<Invitation>.Success(new Invitation(id, createdOnUtc, modifiedOnUtc, inviterId, groupId, inviteeId,
            invitationStatus));
    }

    public static Result<Invitation> Create(ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId inviterId,
        GroupId groupId, UserId inviteeId)
    {
        var id = InvitationId.Create();
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Invitation>.Invalid(result.Errors);
        }

        return Result<Invitation>.Success(new Invitation(id, createdOnUtc, modifiedOnUtc, inviterId, groupId, inviteeId,
            InvitationStatus.Pending));
    }

    public bool Accept(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null || InvitationStatus != InvitationStatus.Pending)
        {
            return false;
        }

        InvitationStatus = InvitationStatus.Accepted;
        UpdateModifiedOnUtc(dateTimeProvider);

        AddDomainEvent(new InvitationAcceptedEvent(dateTimeProvider.UtcNow, Id, InviteeId, GroupId));

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