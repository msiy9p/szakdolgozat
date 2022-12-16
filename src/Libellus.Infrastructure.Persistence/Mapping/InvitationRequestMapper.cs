using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Models;
using DomainInvitationRequest = Libellus.Domain.Entities.InvitationRequest;
using PersistenceInvitationRequest = Libellus.Infrastructure.Persistence.DataModels.InvitationRequest;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct InvitationRequestMapper :
    IMapFrom<PersistenceInvitationRequest, Result<DomainInvitationRequest>>,
    IMapFrom<DomainInvitationRequest, PersistenceInvitationRequest>
{
    public static Result<DomainInvitationRequest> Map(PersistenceInvitationRequest item1)
    {
        return DomainInvitationRequest.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.RequesterId,
            item1.GroupId,
            item1.InvitationStatus);
    }

    public static PersistenceInvitationRequest Map(DomainInvitationRequest item1)
    {
        return new PersistenceInvitationRequest()
        {
            Id = item1.Id,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc,
            RequesterId = item1.RequesterId,
            GroupId = item1.GroupId,
            InvitationStatus = item1.InvitationStatus
        };
    }
}