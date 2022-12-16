using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class SameCreatorRequirement : IAuthorisationRequirement
{
    public UserId? UserId { get; init; }

    public SameCreatorRequirement(UserId? userId)
    {
        UserId = userId;
    }

    public sealed class SameCreatorRequirementHandler :
        BaseRequirementHandler<SameCreatorRequirementHandler>,
        IAuthorisationHandler<SameCreatorRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;

        public SameCreatorRequirementHandler(IIdentityService identityService,
            ILogger<SameCreatorRequirementHandler> logger, ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(SameCreatorRequirement requirement,
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

            return Result.Error(Error.None);
        }
    }
}