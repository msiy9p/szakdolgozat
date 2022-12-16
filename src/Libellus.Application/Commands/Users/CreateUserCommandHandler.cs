using Libellus.Application.Commands.Users.CreateUser;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Commands.Users;

public sealed class CreateUserCommandHandler :
    ICommandHandler<CreateUserCommand, UserId>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommand> _logger;

    public CreateUserCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork,
        ILogger<CreateUserCommand> logger)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userResult =
            await _identityService.CreateUserAsync(request.Email, request.UserName, request.Password,
                request.CallbackUrlTemplate, cancellationToken);
        if (userResult.IsError)
        {
            return Result<UserId>.Error(userResult.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            _logger.LogError(saveResult.Errors);
            return Result<UserId>.Error(saveResult.Errors);
        }

        return Result<UserId>.Success(userResult.Value);
    }
}