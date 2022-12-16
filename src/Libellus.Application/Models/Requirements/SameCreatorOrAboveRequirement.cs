using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class SameCreatorOrAboveRequirement : IAuthorisationRequirement
{
    public UserId? UserId { get; init; }

    public SameCreatorOrAboveRequirement(UserId? userId)
    {
        UserId = userId;
    }

    public sealed class SameCreatorOrAboveRequirementHandler :
        BaseRequirementHandler<SameCreatorOrAboveRequirementHandler>,
        IAuthorisationHandler<SameCreatorOrAboveRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;

        public SameCreatorOrAboveRequirementHandler(IIdentityService identityService,
            ILogger<SameCreatorOrAboveRequirementHandler> logger, ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(SameCreatorOrAboveRequirement requirement,
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

            if (requirement.UserId.HasValue && requirement.UserId.Value == currentUserId.Value)
            {
                return Result.Succeeded;
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

            return Result.Error(Error.None);
        }
    }
}