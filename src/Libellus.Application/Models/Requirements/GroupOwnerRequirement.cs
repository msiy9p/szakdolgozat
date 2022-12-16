using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class GroupOwnerRequirement : IAuthorisationRequirement
{
    public UserId UserId { get; init; }

    public GroupOwnerRequirement(UserId userId)
    {
        UserId = userId;
    }

    public sealed class GroupOwnerRequirementHandler :
        BaseRequirementHandler<GroupOwnerRequirementHandler>,
        IAuthorisationHandler<GroupOwnerRequirement>
    {
        private readonly ICurrentGroupService _currentGroupService;

        public GroupOwnerRequirementHandler(IIdentityService identityService,
            ILogger<GroupOwnerRequirementHandler> logger,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(GroupOwnerRequirement requirement,
            CancellationToken cancellationToken = default)
        {
            var userExists = await _identityService.ExistsAsync(requirement.UserId, cancellationToken);
            if (userExists.IsError || !userExists.Value)
            {
                return SecurityConstants.AuthorisationResults.CurrentUserNotFound;
            }

            var currentGroupId = _currentGroupService.CurrentGroupId;
            if (!currentGroupId.HasValue)
            {
                return SecurityConstants.AuthorisationResults.CurrentGroupNotFound;
            }

            var ownerResult = await _identityService.IsInGroupRoleAsync(requirement.UserId, currentGroupId.Value,
                SecurityConstants.GroupRoles.Owner, cancellationToken);
            if (ownerResult.IsSuccess && ownerResult.Value)
            {
                return Result.Succeeded;
            }

            return SecurityConstants.AuthorisationResults.NotAnOwner;
        }
    }
}