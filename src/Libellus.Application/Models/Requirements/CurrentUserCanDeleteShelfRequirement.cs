using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class CurrentUserCanDeleteShelfRequirement : IAuthorisationRequirement
{
    public ShelfId ShelfId { get; init; }

    public CurrentUserCanDeleteShelfRequirement(ShelfId shelfId)
    {
        ShelfId = shelfId;
    }

    public sealed class CurrentUserCanDeleteShelfRequirementHandler :
        BaseRequirementHandler<CurrentUserCanDeleteShelfRequirementHandler>,
        IAuthorisationHandler<CurrentUserCanDeleteShelfRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;
        private readonly IShelfReadOnlyRepository _shelfReadOnlyRepository;

        public CurrentUserCanDeleteShelfRequirementHandler(IIdentityService identityService,
            ILogger<CurrentUserCanDeleteShelfRequirementHandler> logger, ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService, IShelfReadOnlyRepository shelfReadOnlyRepository) : base(
            identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
            _shelfReadOnlyRepository = shelfReadOnlyRepository;
        }

        public async Task<Result> Handle(CurrentUserCanDeleteShelfRequirement requirement,
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

            var memberResult =
                await _identityService.IsInGroupAsync(currentUserId.Value, currentGroupId.Value, cancellationToken);

            if (memberResult.IsError || !memberResult.Value)
            {
                return SecurityConstants.AuthorisationResults.NotInCurrentGroup;
            }

            var shelf = await _shelfReadOnlyRepository.GetByIdAsync(requirement.ShelfId, cancellationToken);
            if (shelf.IsError)
            {
                return Result.Error(Error.None);
            }

            if (shelf.Value.IsLocked && shelf.Value.CreatorId.HasValue &&
                shelf.Value.CreatorId.Value == currentUserId.Value)
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

            return SecurityConstants.AuthorisationResults.NotModeratorOrAbove;
        }
    }
}