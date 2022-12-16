using Libellus.Application.Common.Interfaces;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Utilities.Measuring;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const double WarningDurationThreshold = 5;

    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentGroupService _currentGroupService;
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ICurrentUserService currentUserService,
        ICurrentGroupService currentGroupService,
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _currentGroupService = currentGroupService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var groupId = _currentGroupService.CurrentGroupId;
        var requestName = typeof(TRequest).Name;
        var requestType = Utilities.Utilities.GetRequestType(request);

        _logger.LogInformation("Handling {RequestType}: {Name} issued by {UserId} with {GroupId}.", requestType,
            requestName, userId.HasValue ? userId.Value.ToString() : "NO_USER_ID",
            groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

        var timer = StopwatchTimer.StartNew();

        var response = await next();

        timer.Stop();

        if (timer.Elapsed.TotalSeconds >= WarningDurationThreshold)
        {
            _logger.LogWarning("Handled {Name} in {Elapsed}.", requestName, timer.Elapsed);
        }
        else
        {
            _logger.LogInformation("Handled {Name} in {Elapsed}.", requestName, timer.Elapsed);
        }

        return response;
    }
}