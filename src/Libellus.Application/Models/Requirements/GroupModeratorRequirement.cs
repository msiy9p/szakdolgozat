using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class GroupModeratorRequirement : IAuthorisationRequirement
{
    public UserId UserId { get; init; }

    public GroupModeratorRequirement(UserId userId)
    {
        UserId = userId;
    }

    public sealed class GroupModeratorRequirementHandler :
        BaseRequirementHandler<GroupModeratorRequirementHandler>,
        IAuthorisationHandler<GroupModeratorRequirement>
    {
        private readonly ICurrentGroupService _currentGroupService;

        public GroupModeratorRequirementHandler(IIdentityService identityService,
            ILogger<GroupModeratorRequirementHandler> logger,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(GroupModeratorRequirement requirement,
            CancellationToken cancellationToken = default)
        {
            var userExists = await _identityService.ExistsAsync(requirement.UserId, cancellationToken);
            if (userExists.IsError || !userExists.Value)
            {
                return SecurityConstants.AuthorisationResults.NoUserFound;
            }

            var currentGroupId = _currentGroupService.CurrentGroupId;
            if (!currentGroupId.HasValue)
            {
                return SecurityConstants.AuthorisationResults.CurrentGroupNotFound;
            }

            var moderatorResult = await _identityService.IsInGroupRoleAsync(requirement.UserId, currentGroupId.Value,
                SecurityConstants.GroupRoles.Moderator, cancellationToken);
            if (moderatorResult.IsSuccess && moderatorResult.Value)
            {
                return Result.Succeeded;
            }

            return SecurityConstants.AuthorisationResults.NotAModerator;
        }
    }
}