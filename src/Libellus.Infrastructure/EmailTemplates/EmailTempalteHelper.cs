using System.Reflection;

namespace Libellus.Infrastructure.EmailTemplates;

internal static class EmailTemplateHelper
{
    public static readonly Assembly Assembly = typeof(EmailTemplateHelper).Assembly;

    public const string BookEditionReleasingTemplate =
        "Libellus.Infrastructure.EmailTemplates.BookEditionReleasing.liquid";

    public const string InvitationTemplate =
        "Libellus.Infrastructure.EmailTemplates.Invitation.liquid";

    public const string EmailConfirmationTemplate =
        "Libellus.Infrastructure.EmailTemplates.EmailConfirmation.liquid";

    public const string ChangeEmailTemplate =
        "Libellus.Infrastructure.EmailTemplates.ChangeEmail.liquid";

    public const string ResetPasswordTemplate =
        "Libellus.Infrastructure.EmailTemplates.ResetPassword.liquid";
}