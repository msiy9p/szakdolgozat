using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Models;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Events;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using Extensions = Libellus.Infrastructure.Persistence.Utilities.Extensions;

namespace Libellus.Infrastructure.Persistence.Services;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly ApplicationContext _context;
    private readonly ILogger<IdentityService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public IdentityService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        ApplicationContext context,
        ILogger<IdentityService> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _context = context;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> ExistsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var found = await _userManager.Users.AnyAsync(x => x.Id == userId, cancellationToken);

        return found.ToResult();
    }

    public async Task<Result<bool>> IsInRoleAsync(UserId userId, string role,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<bool>();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var roleFound = await _userManager.IsInRoleAsync(foundUser, role);

        return roleFound.ToResult();
    }

    public async Task<Result<bool>> AuthorizeWithPolicyAsync(UserId userId, string policyName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<bool>();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(foundUser);

        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, policyName);

        return authorizationResult.Succeeded.ToResult();
    }

    public async Task<Result<bool>> CanViewGroupAsync(UserId userId, GroupId groupId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.AnyAsync(x => x.Id == userId, cancellationToken);
        if (!foundUser)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var foundGroup = await _context.Groups.AnyAsync(x => x.Id == groupId, cancellationToken);
        if (!foundGroup)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<bool>();
        }

        var isPrivate = await _context.Groups.Where(x => x.Id == groupId)
            .Select(x => x.IsPrivate)
            .FirstOrDefaultAsync(cancellationToken);

        if (isPrivate)
        {
            var found = await _context.GroupUserMemberships.Where(x => x.GroupId == groupId)
                .AnyAsync(x => x.UserId == userId, cancellationToken);

            found.ToResult();
        }

        return true.ToResult();
    }

    public async Task<Result<bool>> IsInGroupAsync(UserId userId, GroupId groupId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.AnyAsync(x => x.Id == userId, cancellationToken);
        if (!foundUser)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var foundGroup = await _context.Groups.AnyAsync(x => x.Id == groupId, cancellationToken);
        if (!foundGroup)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<bool>();
        }

        var found = await _context.GroupUserMemberships.Where(x => x.GroupId == groupId)
            .AnyAsync(x => x.UserId == userId, cancellationToken);

        return found.ToResult();
    }

    public async Task<Result<bool>> IsInGroupRoleAsync(UserId userId, GroupId groupId, string role,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<bool>();
        }

        var foundUser = await _userManager.Users.AnyAsync(x => x.Id == userId, cancellationToken);
        if (!foundUser)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var foundGroup = await _context.Groups.AnyAsync(x => x.Id == groupId, cancellationToken);
        if (!foundGroup)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<bool>();
        }

        var found = await _context.GroupUserMemberships
            .Include(x => x.GroupRole)
            .Where(x => x.GroupId == groupId)
            .Where(x => x.UserId == userId)
            .AnyAsync(x => x.GroupRole.Name == role, cancellationToken);

        return found.ToResult();
    }

    public async Task<Result<UserId>> CreateUserAsync(string email, string userName, string password,
        EmailConfirmationCallbackUrlTemplate callbackUrlTemplate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<UserId>();
        }

        var user = new ApplicationUser()
        {
            Id = UserId.Create(),
            Email = email,
            UserName = userName
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result<UserId>.Error(errors);
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, SecurityConstants.IdentityRoles.User);
        if (!addToRoleResult.Succeeded)
        {
            var errors = addToRoleResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result<UserId>.Error(errors);
        }

        var emailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _context._domainEventContainer.GatherEvent(new EmailConfirmationNeededEvent(_dateTimeProvider.UtcNow, user.Id,
            callbackUrlTemplate.CreateUrl(user.Id, Extensions.Base64UrlEncode(emailConfirmation))));

        return user.Id.ToResult();
    }

    public async Task<Result> DeleteUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var operationResult = await _userManager.DeleteAsync(foundUser);

        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        await _signInManager.SignOutAsync();

        return Result.Success();
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<string>();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<string>();
        }

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(foundUser);
        if (!isEmailConfirmed)
        {
            return UserErrors.UserEmailNotConfirmed.ToErrorResult<string>();
        }

        var operationResult = await _userManager.GeneratePasswordResetTokenAsync(foundUser);

        return Extensions.Base64UrlEncode(operationResult).ToResult();
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<string>();
        }

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(foundUser);
        if (!isEmailConfirmed)
        {
            return UserErrors.UserEmailNotConfirmed.ToErrorResult<string>();
        }

        var operationResult = await _userManager.GeneratePasswordResetTokenAsync(foundUser);

        return Extensions.Base64UrlEncode(operationResult).ToResult();
    }

    public async Task<Result> ResetPasswordAsync(UserId userId, string resetToken, string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(resetToken) || string.IsNullOrWhiteSpace(newPassword))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult =
            await _userManager.ResetPasswordAsync(foundUser, Extensions.Base64UrlDecode(resetToken), newPassword);
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string resetToken, string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(resetToken) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult =
            await _userManager.ResetPasswordAsync(foundUser, Extensions.Base64UrlDecode(resetToken), newPassword);
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return Result.Success();
    }

    public async Task<Result<string>> GenerateChangeEmailTokenAsync(UserId userId, string newEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<string>();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<string>();
        }

        var result = await _userManager.FindByEmailAsync(newEmail);
        if (result is not null)
        {
            return UserErrors.UserEmailTaken.ToErrorResult<string>();
        }

        var operationResult = await _userManager.GenerateChangeEmailTokenAsync(foundUser, newEmail);

        return Extensions.Base64UrlEncode(operationResult).ToResult();
    }

    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<string>();
        }

        var operationResult = await _userManager.GenerateEmailConfirmationTokenAsync(foundUser);

        return Extensions.Base64UrlEncode(operationResult).ToResult();
    }

    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<string>();
        }

        var operationResult = await _userManager.GenerateEmailConfirmationTokenAsync(foundUser);

        return Extensions.Base64UrlEncode(operationResult).ToResult();
    }

    public async Task<Result> ChangePasswordAsync(UserId userId, string oldPassword, string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult = await _userManager.ChangePasswordAsync(foundUser, oldPassword, newPassword);

        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return await RefreshSignInAsync(foundUser, cancellationToken);
    }

    public async Task<Result> ChangeEmailAsync(UserId userId, string newEmail, string changeEmailToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newEmail) || string.IsNullOrWhiteSpace(changeEmailToken))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult =
            await _userManager.ChangeEmailAsync(foundUser, newEmail, Extensions.Base64UrlDecode(changeEmailToken));
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return await RefreshSignInAsync(foundUser, cancellationToken);
    }

    public async Task<Result> ConfirmEmailAsync(UserId userId, string emailToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailToken))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult = await _userManager.ConfirmEmailAsync(foundUser, Extensions.Base64UrlDecode(emailToken));
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return Result.Success();
    }

    public async Task<Result> PasswordSignInAsync(string email, string password, bool rememberMe,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult();
        }

        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult = await _signInManager.PasswordSignInAsync(foundUser, password, rememberMe, true);
        if (!operationResult.Succeeded)
        {
            if (operationResult.IsNotAllowed)
            {
                return UserErrors.UserLoginIsNotAllowed.ToErrorResult();
            }

            if (operationResult.IsLockedOut)
            {
                return UserErrors.UserIsLockedOut.ToErrorResult();
            }

            if (operationResult.RequiresTwoFactor)
            {
                return UserErrors.UserLoginRequiresTwoFactor.ToErrorResult();
            }

            return Error.None.ToErrorResult();
        }

        return Result.Success();
    }

    public async Task<Result<bool>> IsTwoFactorEnabledAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var value = await _userManager.GetTwoFactorEnabledAsync(foundUser);

        return value.ToResult();
    }

    public async Task<Result<bool>> VerifyTwoFactorTokenAsync(UserId userId, string twoFactorCode,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var value = await _userManager.VerifyTwoFactorTokenAsync(foundUser,
            _userManager.Options.Tokens.AuthenticatorTokenProvider, twoFactorCode);

        return value.ToResult();
    }

    public async Task<Result<string>> GetAuthenticatorKeyAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<string>();
        }

        var key = await _userManager.GetAuthenticatorKeyAsync(foundUser);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(foundUser);
            key = await _userManager.GetAuthenticatorKeyAsync(foundUser);
        }

        return key!.ToResult();
    }

    public async Task<Result> ResetAuthenticatorKey(UserId userId, CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        var operationResult = await _userManager.SetTwoFactorEnabledAsync(foundUser, false);
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        operationResult = await _userManager.ResetAuthenticatorKeyAsync(foundUser);
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return await RefreshSignInAsync(foundUser, cancellationToken);
    }

    public async Task<Result<IReadOnlyCollection<string>>> EnableTwoFactorAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<IReadOnlyCollection<string>>();
        }

        var operationResult = await _userManager.SetTwoFactorEnabledAsync(foundUser, enabled: true);
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result<IReadOnlyCollection<string>>.Error(errors);
        }

        if (await _userManager.CountRecoveryCodesAsync(foundUser) == 0)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(foundUser, 10);
            IReadOnlyCollection<string> output = recoveryCodes!.ToList();
            return output.ToResult();
        }

        IReadOnlyCollection<string> output1 = Array.Empty<string>();
        return output1.ToResult();
    }

    public async Task<Result<IReadOnlyCollection<string>>> GenerateNewTwoFactorRecoveryCodesAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<IReadOnlyCollection<string>>();
        }

        var value = await _userManager.GetTwoFactorEnabledAsync(foundUser);
        if (!value)
        {
            return UserErrors.TwoFactorIsDisabled.ToErrorResult<IReadOnlyCollection<string>>();
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(foundUser, 10);
        IReadOnlyCollection<string> output = recoveryCodes!.ToList();
        return output.ToResult();
    }

    public async Task<Result> DisableTwoFactorAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<bool>();
        }

        var operationResult = await _userManager.SetTwoFactorEnabledAsync(foundUser, enabled: false);
        if (!operationResult.Succeeded)
        {
            var errors = operationResult.Errors
                .Select(identityError => new Error(identityError.Code, identityError.Description))
                .ToList();

            return Result.Error(errors);
        }

        return Result.Success();
    }

    public async Task<Result> TwoFactorSignInAsync(string twoFactorCode, bool rememberMe, bool rememberMachine,
        CancellationToken cancellationToken = default)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
        {
            return UserErrors.UserLoginRequiresTwoFactor.ToErrorResult();
        }

        var authCode = twoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var operationResult =
            await _signInManager.TwoFactorAuthenticatorSignInAsync(authCode, rememberMe, rememberMachine);
        if (!operationResult.Succeeded)
        {
            if (operationResult.IsNotAllowed)
            {
                return UserErrors.UserLoginIsNotAllowed.ToErrorResult();
            }

            if (operationResult.IsLockedOut)
            {
                return UserErrors.UserIsLockedOut.ToErrorResult();
            }

            return UserErrors.TwoFactorCodeNotValid.ToErrorResult();
        }

        return Result.Success();
    }

    public async Task<Result> RecoveryCodeSignInAsync(string recoveryCode,
        CancellationToken cancellationToken = default)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
        {
            return UserErrors.UserLoginRequiresTwoFactor.ToErrorResult();
        }

        var code = recoveryCode.Replace(" ", string.Empty);

        var operationResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);
        if (!operationResult.Succeeded)
        {
            if (operationResult.IsNotAllowed)
            {
                return UserErrors.UserLoginIsNotAllowed.ToErrorResult();
            }

            if (operationResult.IsLockedOut)
            {
                return UserErrors.UserIsLockedOut.ToErrorResult();
            }

            return UserErrors.RecoveryCodeNotValid.ToErrorResult();
        }

        return Result.Success();
    }

    public async Task<Result<bool>> IsInTwoFactorAuthenticationPhaseAsync(CancellationToken cancellationToken = default)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
        {
            return false.ToResult();
        }

        return true.ToResult();
    }

    public async Task<Result<TwoFactorSummaryVm>> GetTwoFactorSummaryAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<TwoFactorSummaryVm>();
        }

        var hasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(foundUser) != null;
        var is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(foundUser);
        var isMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(foundUser);
        var recoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(foundUser);

        return new TwoFactorSummaryVm(hasAuthenticator, recoveryCodesLeft, is2FaEnabled, isMachineRemembered)
            .ToResult();
    }

    public async Task<Result> ForgetTwoFactorClientAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.ForgetTwoFactorClientAsync();

        return Result.Success();
    }

    public async Task<Result> SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();

        return Result.Success();
    }

    public async Task<Result> RefreshSignInAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var foundUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (foundUser is null)
        {
            return UserErrors.UserNotFound.ToErrorResult();
        }

        return await RefreshSignInAsync(foundUser, cancellationToken);
    }

    private async Task<Result> RefreshSignInAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await _signInManager.RefreshSignInAsync(user);

        return Result.Success();
    }
}