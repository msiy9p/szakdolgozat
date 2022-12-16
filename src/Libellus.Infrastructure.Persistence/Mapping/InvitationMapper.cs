using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Models;
using DomainInvitation = Libellus.Domain.Entities.Invitation;
using PersistenceInvitation = Libellus.Infrastructure.Persistence.DataModels.Invitation;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct InvitationMapper : IMapFrom<PersistenceInvitation, Result<DomainInvitation>>,
    IMapFrom<DomainInvitation, PersistenceInvitation>
{
    public static Result<DomainInvitation> Map(PersistenceInvitation item1)
    {
        return DomainInvitation.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.InviterId,
            item1.GroupId,
            item1.InviteeId,
            item1.InvitationStatus);
    }

    public static PersistenceInvitation Map(DomainInvitation item1)
    {
        return new PersistenceInvitation()
        {
            Id = item1.Id,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc,
            InviterId = item1.InviterId,
            GroupId = item1.GroupId,
            InviteeId = item1.InviteeId,
            InvitationStatus = item1.InvitationStatus
        };
    }
}