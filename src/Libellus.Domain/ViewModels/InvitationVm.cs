using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Domain.ViewModels;

public sealed class InvitationVm
{
    public InvitationId InvitationId { get; init; }
    public string GroupName { get; init; }
    public string InviterName { get; init; }
    public string InviteeEmail { get; init; }
    public string InviteeName { get; init; }

    public InvitationVm(InvitationId id, string groupName, string inviterName, string inviteeEmail, string inviteeName)
    {
        InvitationId = id;
        GroupName = Guard.Against.NullOrWhiteSpace(groupName);
        InviterName = Guard.Against.NullOrWhiteSpace(inviterName);
        InviteeEmail = Guard.Against.NullOrWhiteSpace(inviteeEmail);
        InviteeName = Guard.Against.NullOrWhiteSpace(inviteeName);
    }

    public static Result<InvitationVm> Create(InvitationId id, string groupName, string inviterName,
        string inviteeEmail, string inviteeName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<InvitationVm>();
        }

        if (string.IsNullOrWhiteSpace(inviterName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<InvitationVm>();
        }

        if (string.IsNullOrWhiteSpace(inviteeEmail))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<InvitationVm>();
        }

        if (string.IsNullOrWhiteSpace(inviteeName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<InvitationVm>();
        }

        return new InvitationVm(id, groupName, inviterName, inviteeEmail, inviteeName).ToResult();
    }
}