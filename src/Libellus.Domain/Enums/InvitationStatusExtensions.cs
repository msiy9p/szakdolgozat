namespace Libellus.Domain.Enums;

public static class InvitationStatusExtensions
{
    public static bool IsDefined(InvitationStatus invitationStatus) =>
        invitationStatus switch
        {
            InvitationStatus.Expired => true,
            InvitationStatus.Pending => true,
            InvitationStatus.Declined => true,
            InvitationStatus.Accepted => true,
            _ => false
        };

    public static string ToString(InvitationStatus invitationStatus) =>
        invitationStatus switch
        {
            InvitationStatus.Expired => nameof(InvitationStatus.Expired),
            InvitationStatus.Pending => nameof(InvitationStatus.Pending),
            InvitationStatus.Declined => nameof(InvitationStatus.Declined),
            InvitationStatus.Accepted => nameof(InvitationStatus.Accepted),
            _ => string.Empty
        };
}