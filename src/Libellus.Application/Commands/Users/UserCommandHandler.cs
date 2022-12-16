using Libellus.Application.Commands.ProfilePictures.DeleteProfilePicturesById;
using Libellus.Application.Commands.Users.ChangeUserEmail;
using Libellus.Application.Commands.Users.ChangeUserPassword;
using Libellus.Application.Commands.Users.ChangeUserProfilePicture;
using Libellus.Application.Commands.Users.ConfirmUserEmail;
using Libellus.Application.Commands.Users.DeleteUserById;
using Libellus.Application.Commands.Users.DeleteUserProfilePicture;
using Libellus.Application.Commands.Users.DisableTwoFactorById;
using Libellus.Application.Commands.Users.EnableTwoFactorById;
using Libellus.Application.Commands.Users.ForgetTwoFactorClient;
using Libellus.Application.Commands.Users.GenerateChangeUserEmailToken;
using Libellus.Application.Commands.Users.GenerateNewTwoFactorRecoveryCodes;
using Libellus.Application.Commands.Users.GenerateUserEmailConfirmationToken;
using Libellus.Application.Commands.Users.GenerateUserPasswordResetToken;
using Libellus.Application.Commands.Users.GenerateUserPasswordResetTokenByEmail;
using Libellus.Application.Commands.Users.PasswordSignIn;
using Libellus.Application.Commands.Users.RecoveryCodeSignIn;
using Libellus.Application.Commands.Users.ResetAuthenticatorKeyById;
using Libellus.Application.Commands.Users.ResetUserPassword;
using Libellus.Application.Commands.Users.ResetUserPasswordByEmail;
using Libellus.Application.Commands.Users.SignOut;
using Libellus.Application.Commands.Users.TwoFactorSignIn;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Commands.Users;

public sealed class UserCommandHandler :
    ICommandHandler<GenerateChangeUserEmailTokenCommand>,
    ICommandHandler<GenerateUserPasswordResetTokenCommand>,
    ICommandHandler<ResetUserPasswordCommand>,
    ICommandHandler<ResetUserPasswordByEmailCommand>,
    ICommandHandler<ChangeUserPasswordCommand>,
    ICommandHandler<ChangeUserEmailCommand>,
    ICommandHandler<ConfirmUserEmailCommand>,
    ICommandHandler<GenerateUserEmailConfirmationTokenCommand>,
    ICommandHandler<SignOutCommand>,
    ICommandHandler<PasswordSignInCommand>,
    ICommandHandler<TwoFactorSignInCommand>,
    ICommandHandler<RecoveryCodeSignInCommand>,
    ICommandHandler<GenerateUserPasswordResetTokenByEmailCommand>,
    ICommandHandler<DeleteUserByIdCommand>,
    ICommandHandler<EnableTwoFactorByIdCommand, IReadOnlyCollection<string>>,
    ICommandHandler<GenerateNewTwoFactorRecoveryCodesCommand, IReadOnlyCollection<string>>,
    ICommandHandler<DisableTwoFactorByIdCommand>,
    ICommandHandler<ResetAuthenticatorKeyByIdCommand>,
    ICommandHandler<ForgetTwoFactorClientCommand>,
    ICommandHandler<ChangeUserProfilePictureCommand>,
    ICommandHandler<DeleteUserProfilePictureCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<UserCommandHandler> _logger;

    public UserCommandHandler(IIdentityService identityService, IUserRepository userRepository,
        IEmailService emailService, IUnitOfWork unitOfWork, ILogger<UserCommandHandler> logger, ISender sender)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _sender = sender;
    }

    public async Task<Result> Handle(GenerateChangeUserEmailTokenCommand request, CancellationToken cancellationToken)
    {
        var userResult =
            await _identityService.GenerateChangeEmailTokenAsync(request.UserId, request.NewEmail, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var userVm = await _userRepository.GetEmailVmByIdAsync(request.UserId, cancellationToken);
        if (userVm.IsError)
        {
            return Result.Error(userVm.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        await _emailService.SendChangeEmailTokenAsync(userVm.Value!,
            request.EmailConfirmationCallbackUrlTemplate.CreateUrl(request.UserId, userResult.Value!),
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(GenerateUserPasswordResetTokenCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.GeneratePasswordResetTokenAsync(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var userVm = await _userRepository.GetEmailVmByIdAsync(request.UserId, cancellationToken);
        if (userVm.IsError)
        {
            return Result.Error(userVm.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        await _emailService.SendResetPasswordTokenAsync(userVm.Value!,
            request.ForgotPasswordCallbackUrlTemplate.CreateUrl(userResult.Value!), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.ResetPasswordAsync(request.UserId, request.ResetToken,
            request.NewPassword, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.ChangePasswordAsync(request.UserId, request.OldPassword,
            request.NewPassword, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(ChangeUserEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult =
            await _identityService.ChangeEmailAsync(request.UserId, request.NewEmail, request.Token, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult =
            await _identityService.ConfirmEmailAsync(request.UserId, request.EmailToken, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(GenerateUserEmailConfirmationTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await _identityService.GenerateEmailConfirmationTokenAsync(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var userVm = await _userRepository.GetEmailVmByIdAsync(request.UserId, cancellationToken);
        if (userVm.IsError)
        {
            return Result.Error(userVm.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        await _emailService.SendEmailConfirmationAsync(userVm.Value!,
            request.EmailConfirmationCallbackUrlTemplate.CreateUrl(request.UserId, userResult.Value!),
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(ResetUserPasswordByEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.ResetPasswordAsync(request.Email, request.ResetToken,
            request.NewPassword, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SignOutAsync(cancellationToken);
    }

    public async Task<Result> Handle(PasswordSignInCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.PasswordSignInAsync(request.Email, request.Password, request.RememberMe,
            cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(TwoFactorSignInCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.TwoFactorSignInAsync(request.TwoFactorCode, request.RememberMe,
            request.RememberMachine, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(RecoveryCodeSignInCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.RecoveryCodeSignInAsync(request.RecoveryCode, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(GenerateUserPasswordResetTokenByEmailCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await _identityService.GenerateEmailConfirmationTokenAsync(request.Email, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var userId = await _userRepository.GetIdByEmailAsync(request.Email, cancellationToken);
        if (userId.IsError)
        {
            return Result.Error(userId.Errors);
        }

        var userVm = await _userRepository.GetEmailVmByIdAsync(userId.Value, cancellationToken);
        if (userVm.IsError)
        {
            return Result.Error(userVm.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        await _emailService.SendEmailConfirmationAsync(userVm.Value!,
            request.ForgotPasswordCallbackUrlTemplate.CreateUrl(userResult.Value!), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.DeleteUserAsync(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<string>>> Handle(EnableTwoFactorByIdCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await _identityService.EnableTwoFactorAsync(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result<IReadOnlyCollection<string>>.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<IReadOnlyCollection<string>>.Error(saveResult.Errors);
        }

        return userResult;
    }

    public async Task<Result> Handle(DisableTwoFactorByIdCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.DisableTwoFactorAsync(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(ResetAuthenticatorKeyByIdCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.ResetAuthenticatorKey(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result.Error(saveResult.Errors);
        }

        return Result.Success();
    }

    public async Task<Result> Handle(ForgetTwoFactorClientCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ForgetTwoFactorClientAsync(cancellationToken);
    }

    public async Task<Result<IReadOnlyCollection<string>>> Handle(GenerateNewTwoFactorRecoveryCodesCommand request,
        CancellationToken cancellationToken)
    {
        var userResult =
            await _identityService.GenerateNewTwoFactorRecoveryCodesAsync(request.UserId, cancellationToken);
        if (userResult.IsError)
        {
            return Result<IReadOnlyCollection<string>>.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<IReadOnlyCollection<string>>.Error(saveResult.Errors);
        }

        return userResult;
    }

    public async Task<Result> Handle(ChangeUserProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var result =
            await _userRepository.ChangeProfilePictureAsync(request.UserId, request.ProfilePictureId,
                cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteUserProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var imgId = await _userRepository.GetPictureIdAsync(request.UserId, cancellationToken);
        if (imgId.IsError)
        {
            return imgId;
        }

        if (!imgId.Value.HasValue)
        {
            return Result.Success();
        }

        var result = await _userRepository.RemoveProfilePictureAsync(request.UserId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var command = new DeleteProfilePicturesByIdCommand(imgId.Value.Value);
        var commandResult = await _sender.Send(command, cancellationToken);
        if (commandResult.IsError)
        {
            return commandResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}