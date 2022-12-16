using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Models;
using Libellus.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class FailAuthorisationRequirement : IAuthorisationRequirement
{
    public static readonly FailAuthorisationRequirement Instance = new();

    private FailAuthorisationRequirement()
    {
    }

    public sealed class FailAuthorisationRequirementHandler :
        BaseRequirementHandler<FailAuthorisationRequirementHandler>,
        IAuthorisationHandler<FailAuthorisationRequirement>
    {
        public FailAuthorisationRequirementHandler(IIdentityService identityService,
            ILogger<FailAuthorisationRequirementHandler> logger) : base(identityService, logger)
        {
        }

        public async Task<Result> Handle(FailAuthorisationRequirement requirement,
            CancellationToken cancellationToken = default)
        {
            return Result.Error(Error.None);
        }
    }
}