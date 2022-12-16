using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class NotInCurrentGroupRequirement : IAuthorisationRequirement
{
    public UserId UserId { get; init; }

    public NotInCurrentGroupRequirement(UserId userId)
    {
        UserId = userId;
    }

    public sealed class NotInCurrentGroupRequirementHandler :
        BaseRequirementHandler<NotInCurrentGroupRequirementHandler>,
        IAuthorisationHandler<NotInCurrentGroupRequirement>
    {
        private readonly ICurrentGroupService _currentGroupService;

        public NotInCurrentGroupRequirementHandler(IIdentityService identityService,
            ILogger<NotInCurrentGroupRequirementHandler> logger,
            ICurrentGroupService currentGroupService) : base(identityService, logger)
        {
            _currentGroupService = currentGroupService;
        }

        public async Task<Result> Handle(NotInCurrentGroupRequirement requirement,
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

            var result =
                await _identityService.IsInGroupAsync(requirement.UserId, currentGroupId.Value, cancellationToken);

            if (result.IsSuccess && !result.Value)
            {
                return Result.Succeeded;
            }

            return Result.Error(Error.None);
        }
    }
}