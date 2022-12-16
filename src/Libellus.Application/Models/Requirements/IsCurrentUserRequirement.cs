using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class IsCurrentUserRequirement : IAuthorisationRequirement
{
    public UserId UserId { get; init; }

    public IsCurrentUserRequirement(UserId userId)
    {
        UserId = userId;
    }

    public sealed class IsCurrentUserRequirementHandler : BaseRequirementHandler<IsCurrentUserRequirementHandler>,
        IAuthorisationHandler<IsCurrentUserRequirement>
    {
        private readonly ICurrentUserService _currentUserService;

        public IsCurrentUserRequirementHandler(IIdentityService identityService,
            ILogger<IsCurrentUserRequirementHandler> logger,
            ICurrentUserService currentUserService) : base(identityService, logger)
        {
            _currentUserService = currentUserService;
        }

        public Task<Result> Handle(IsCurrentUserRequirement requirement,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return Task.FromResult(SecurityConstants.AuthorisationResults.CurrentUserNotFound);
            }

            if (currentUserId.Value == requirement.UserId)
            {
                return Task.FromResult(Result.Succeeded);
            }

            return Task.FromResult(SecurityConstants.AuthorisationResults.NotTheSameUser);
        }
    }
}