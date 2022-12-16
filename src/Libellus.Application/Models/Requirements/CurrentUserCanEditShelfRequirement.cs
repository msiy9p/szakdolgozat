using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class CurrentUserCanEditShelfRequirement : IAuthorisationRequirement
{
    public ShelfId ShelfId { get; init; }

    public CurrentUserCanEditShelfRequirement(ShelfId shelfId)
    {
        ShelfId = shelfId;
    }

    public sealed class CurrentUserCanEditShelfRequirementHandler :
        BaseRequirementHandler<CurrentUserCanEditShelfRequirementHandler>,
        IAuthorisationHandler<CurrentUserCanEditShelfRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;
        private readonly IShelfReadOnlyRepository _shelfReadOnlyRepository;

        public CurrentUserCanEditShelfRequirementHandler(IIdentityService identityService,
            ILogger<CurrentUserCanEditShelfRequirementHandler> logger, ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService, IShelfReadOnlyRepository shelfReadOnlyRepository) : base(
            identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
            _shelfReadOnlyRepository = shelfReadOnlyRepository;
        }

        public async Task<Result> Handle(CurrentUserCanEditShelfRequirement requirement,
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

            var result =
                await _identityService.IsInGroupAsync(currentUserId.Value, currentGroupId.Value, cancellationToken);

            if (result.IsError || !result.Value)
            {
                return SecurityConstants.AuthorisationResults.NotInCurrentGroup;
            }

            var shelf = await _shelfReadOnlyRepository.GetByIdAsync(requirement.ShelfId, cancellationToken);
            if (shelf.IsError)
            {
                return Result.Error(Error.None);
            }

            if (!shelf.Value.IsLocked)
            {
                return Result.Succeeded;
            }

            if (shelf.Value.CreatorId.HasValue && shelf.Value.CreatorId.Value == currentUserId.Value)
            {
                return Result.Succeeded;
            }

            return SecurityConstants.AuthorisationResults.NotCreatorOfResource;
        }
    }
}