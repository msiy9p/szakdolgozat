using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class GroupMemberRequirement : IAuthorisationRequirement
{
    public UserId UserId { get; init; }

    public GroupMemberRequirement(UserId userId)
    {
        UserId = userId;
    }

    public sealed class GroupMemberRequirementHandler :
        BaseRequirementHandler<GroupMemberRequirementHandler>,
        IAuthorisationHandler<GroupMemberRequirement>
    {
        private readonly ICurrentGroupService _currentGroupService;

        public GroupMemberRequirementHandler(IIdentityService identityService,
            ILogger<GroupMemberRequirementHandler> logger,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(GroupMemberRequirement requirement,
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
                SecurityConstants.GroupRoles.Member, cancellationToken);
            if (ownerResult.IsSuccess && ownerResult.Value)
            {
                return Result.Succeeded;
            }

            return SecurityConstants.AuthorisationResults.NotAMember;
        }
    }
}