using Libellus.Application.Models;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<Result<bool>> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsInRoleAsync(UserId userId, string role, CancellationToken cancellationToken = default);

    Task<Result<bool>> AuthorizeWithPolicyAsync(UserId userId, string policyName,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> CanViewGroupAsync(UserId userId, GroupId groupId, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsInGroupAsync(UserId userId, GroupId groupId, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsInGroupRoleAsync(UserId userId, GroupId groupId, string role,
        CancellationToken cancellationToken = default);

    Task<Result<UserId>> CreateUserAsync(string email, string userName, string password,
        EmailConfirmationCallbackUrlTemplate callbackUrlTemplate, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);

    Task<Result<string>> GeneratePasswordResetTokenAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(UserId userId, string resetToken, string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(string email, string resetToken, string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result<string>> GenerateChangeEmailTokenAsync(UserId userId, string newEmail,
        CancellationToken cancellationToken = default);

    Task<Result<string>> GenerateEmailConfirmationTokenAsync(UserId userId,
        CancellationToken cancellationToken = default);

    Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email,
        CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(UserId userId, string oldPassword, string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result> ChangeEmailAsync(UserId userId, string newEmail, string changeEmailToken,
        CancellationToken cancellationToken = default);

    Task<Result> ConfirmEmailAsync(UserId userId, string emailToken, CancellationToken cancellationToken = default);

    Task<Result> PasswordSignInAsync(string email, string password, bool rememberMe,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> IsTwoFactorEnabledAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<Result<bool>> VerifyTwoFactorTokenAsync(UserId userId, string twoFactorCode,
        CancellationToken cancellationToken = default);

    Task<Result<string>> GetAuthenticatorKeyAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<Result> ResetAuthenticatorKey(UserId userId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<string>>> EnableTwoFactorAsync(UserId userId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<string>>> GenerateNewTwoFactorRecoveryCodesAsync(UserId userId,
        CancellationToken cancellationToken = default);

    Task<Result> DisableTwoFactorAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<Result> TwoFactorSignInAsync(string twoFactorCode, bool rememberMe, bool rememberMachine,
        CancellationToken cancellationToken = default);

    Task<Result> RecoveryCodeSignInAsync(string recoveryCode, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsInTwoFactorAuthenticationPhaseAsync(CancellationToken cancellationToken = default);

    Task<Result<TwoFactorSummaryVm>> GetTwoFactorSummaryAsync(UserId userId,
        CancellationToken cancellationToken = default);

    Task<Result> ForgetTwoFactorClientAsync(CancellationToken cancellationToken = default);

    Task<Result> SignOutAsync(CancellationToken cancellationToken = default);

    Task<Result> RefreshSignInAsync(UserId id, CancellationToken cancellationToken = default);
}