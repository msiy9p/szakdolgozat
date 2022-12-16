using Libellus.Application.Common.Interfaces;
using Libellus.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentGroupService _currentGroupService;
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ICurrentUserService currentUserService, ICurrentGroupService currentGroupService,
        ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _currentGroupService = currentGroupService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var userId = _currentUserService.UserId;
            var groupId = _currentGroupService.CurrentGroupId;
            var requestType = Utilities.Utilities.GetRequestType(request);

            _logger.LogError(ex, "Unhandled Exception for {RequestType} {Name}, {UserId} with {GroupId}.",
                requestType, typeof(TRequest).Name,
                userId.HasValue ? userId.Value.ToString() : "NO_USER_ID",
                groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

            throw;
        }
    }
}