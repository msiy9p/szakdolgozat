using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class CurrentUserAtLeastGroupModeratorRequirement : IAuthorisationRequirement
{
    public static readonly CurrentUserAtLeastGroupModeratorRequirement Instance = new();

    private CurrentUserAtLeastGroupModeratorRequirement()
    {
    }

    public sealed class CurrentUserAtLeastGroupModeratorRequirementHandler :
        BaseRequirementHandler<CurrentUserAtLeastGroupModeratorRequirementHandler>,
        IAuthorisationHandler<CurrentUserAtLeastGroupModeratorRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;

        public CurrentUserAtLeastGroupModeratorRequirementHandler(IIdentityService identityService,
            ILogger<CurrentUserAtLeastGroupModeratorRequirementHandler> logger, ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(CurrentUserAtLeastGroupModeratorRequirement requirement,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return SecurityConstants.AuthorisationResults.CurrentUserNotFound;
            }

            var currentGroupId = _currentGroupService.CurrentGroupId;
            if (!currentGroupId.HasValue)
            {
                return SecurityConstants.AuthorisationResults.CurrentGroupNotFound;
            }

            var moderatorResult = await _identityService.IsInGroupRoleAsync(currentUserId.Value, currentGroupId.Value,
                SecurityConstants.GroupRoles.Moderator, cancellationToken);
            if (moderatorResult.IsSuccess && moderatorResult.Value)
            {
                return Result.Succeeded;
            }

            var ownerResult = await _identityService.IsInGroupRoleAsync(currentUserId.Value, currentGroupId.Value,
                SecurityConstants.GroupRoles.Owner, cancellationToken);
            if (ownerResult.IsSuccess && ownerResult.Value)
            {
                return Result.Succeeded;
            }

            var adminResult = await _identityService.IsInRoleAsync(currentUserId.Value,
                SecurityConstants.IdentityRoles.Administrator, cancellationToken);
            if (adminResult.IsSuccess && adminResult.Value)
            {
                return Result.Succeeded;
            }

            return SecurityConstants.AuthorisationResults.NotModeratorOrAbove;
        }
    }
}