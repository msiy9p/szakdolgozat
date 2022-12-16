using Libellus.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Common.Security.Requirements;

public abstract class BaseRequirementHandler
{
    protected readonly IIdentityService _identityService;

    protected BaseRequirementHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
}

public abstract class BaseRequirementHandler<T> : BaseRequirementHandler where T : BaseRequirementHandler
{
    protected readonly ILogger<T> _logger;

    protected BaseRequirementHandler(IIdentityService identityService, ILogger<T> logger) : base(identityService)
    {
        _logger = logger;
    }
}