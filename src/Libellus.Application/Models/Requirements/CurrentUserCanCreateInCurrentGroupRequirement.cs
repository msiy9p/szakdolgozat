using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class CurrentUserCanCreateInCurrentGroupRequirement : IAuthorisationRequirement
{
    public static readonly CurrentUserCanCreateInCurrentGroupRequirement Instance = new();

    private CurrentUserCanCreateInCurrentGroupRequirement()
    {
    }

    public sealed class CurrentUserCanCreateInCurrentGroupRequirementHandler :
        BaseRequirementHandler<CurrentUserCanCreateInCurrentGroupRequirementHandler>,
        IAuthorisationHandler<CurrentUserCanCreateInCurrentGroupRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentGroupService _currentGroupService;

        public CurrentUserCanCreateInCurrentGroupRequirementHandler(IIdentityService identityService,
            ILogger<CurrentUserCanCreateInCurrentGroupRequirementHandler> logger,
            ICurrentUserService currentUserService,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentUserService = currentUserService;
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(CurrentUserCanCreateInCurrentGroupRequirement requirement,
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

            if (result.IsSuccess && result.Value)
            {
                return Result.Succeeded;
            }

            return SecurityConstants.AuthorisationResults.NotInCurrentGroup;
        }
    }
}