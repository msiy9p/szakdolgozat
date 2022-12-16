using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Application.Common.Security;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class CurrentUserAtLeastGroupOwnerRequirement : IAuthorisationRequirement
{
    public static readonly CurrentUserAtLeastGroupOwnerRequirement Instance = new();

    private CurrentUserAtLeastGroupOwnerRequirement()
    {
    }

    public sealed class CurrentUserAtLeastGroupOwnerRequirementHandler :
        BaseRequirementHandler<CurrentUserAtLeastGroupOwnerRequirementHandler>,
        IAuthorisationHandler<CurrentUserAtLeastGroupOwnerRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;

        public CurrentUserAtLeastGroupOwnerRequirementHandler(IIdentityService identityService,
            ILogger<CurrentUserAtLeastGroupOwnerRequirementHandler> logger,
            ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(CurrentUserAtLeastGroupOwnerRequirement requirement,
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

            return SecurityConstants.AuthorisationResults.NotOwnerOrAbove;
        }
    }
}