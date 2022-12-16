using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Queries.Users.GetAuthenticatorKeyById;
using Libellus.Application.Queries.Users.GetEmailById;
using Libellus.Application.Queries.Users.GetTwoFactorSummaryById;
using Libellus.Application.Queries.Users.GetUserIdByEmail;
using Libellus.Application.Queries.Users.GetUserIdByName;
using Libellus.Application.Queries.Users.GetUserPicturedVmById;
using Libellus.Application.Queries.Users.IsInTwoFactorAuthenticationPhase;
using Libellus.Application.Queries.Users.IsTwoFactorEnabledById;
using Libellus.Application.Queries.Users.IsTwoFactorTokenValid;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Queries.Users;

public sealed class UserQueryHandler :
    IQueryHandler<IsInTwoFactorAuthenticationPhaseQuery, bool>,
    IQueryHandler<GetUserIdByEmailQuery, UserId>,
    IQueryHandler<GetUserIdByNameQuery, UserId>,
    IQueryHandler<IsTwoFactorEnabledByIdQuery, bool>,
    IQueryHandler<GetEmailByIdQuery, string>,
    IQueryHandler<IsTwoFactorTokenValidQuery, bool>,
    IQueryHandler<GetAuthenticatorKeyByIdQuery, string>,
    IQueryHandler<GetTwoFactorSummaryByIdQuery, TwoFactorSummaryVm>,
    IQueryHandler<GetUserPicturedVmByIdQuery, UserPicturedVm>
{
    private readonly IIdentityService _identityService;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly ILogger<UserQueryHandler> _logger;

    public UserQueryHandler(IIdentityService identityService, IUserReadOnlyRepository userRepository,
        ILogger<UserQueryHandler> logger)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(IsInTwoFactorAuthenticationPhaseQuery request,
        CancellationToken cancellationToken)
    {
        return await _identityService.IsInTwoFactorAuthenticationPhaseAsync(cancellationToken);
    }

    public async Task<Result<UserId>> Handle(GetUserIdByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetIdByEmailAsync(request.Email, cancellationToken);
    }

    public async Task<Result<bool>> Handle(IsTwoFactorEnabledByIdQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.IsTwoFactorEnabledAsync(request.UserId, cancellationToken);
    }

    public async Task<Result<string>> Handle(GetEmailByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetEmailVmByIdAsync(request.UserId, cancellationToken);
        if (result.IsError)
        {
            return Result<string>.Error(result.Errors);
        }

        return result.Value!.UserEmail.ToResult();
    }

    public async Task<Result<bool>> Handle(IsTwoFactorTokenValidQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.VerifyTwoFactorTokenAsync(request.UserId, request.TwoFactorCode,
            cancellationToken);
    }

    public async Task<Result<string>> Handle(GetAuthenticatorKeyByIdQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetAuthenticatorKeyAsync(request.UserId, cancellationToken);
    }

    public async Task<Result<TwoFactorSummaryVm>> Handle(GetTwoFactorSummaryByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _identityService.GetTwoFactorSummaryAsync(request.UserId, cancellationToken);
    }

    public async Task<Result<UserPicturedVm>> Handle(GetUserPicturedVmByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _userRepository.GetPicturedVmByIdAsync(request.UserId, cancellationToken);
    }

    public async Task<Result<UserId>> Handle(GetUserIdByNameQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetIdByNameAsync(request.UserName, cancellationToken);
    }
}