using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task<Result> SendInvitationAsync(InvitationVm invitationVm, CancellationToken cancellationToken = default);

    Task<Result> SendEmailConfirmationAsync(UserEmailVm userEmailVm, string emailConfirmationUrl,
        CancellationToken cancellationToken = default);

    Task<Result> SendChangeEmailTokenAsync(UserEmailVm userEmailVm, string changeEmailUrl,
        CancellationToken cancellationToken = default);

    Task<Result> SendResetPasswordTokenAsync(UserEmailVm userEmailVm, string resetTokenUrl,
        CancellationToken cancellationToken = default);
}