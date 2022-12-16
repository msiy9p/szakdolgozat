using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security.Requirements;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Libellus.Application.Models.Requirements;

public sealed class PostNotLockedRequirement : IAuthorisationRequirement
{
    public PostId PostId { get; init; }

    public PostNotLockedRequirement(PostId postId)
    {
        PostId = postId;
    }

    public sealed class PostNotLockedRequirementHandler :
        BaseRequirementHandler<PostNotLockedRequirementHandler>,
        IAuthorisationHandler<PostNotLockedRequirement>
    {
        private readonly IPostReadOnlyRepository _repository;

        public PostNotLockedRequirementHandler(IIdentityService identityService,
            ILogger<PostNotLockedRequirementHandler> logger, IPostReadOnlyRepository repository) : base(identityService,
            logger)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(PostNotLockedRequirement requirement,
            CancellationToken cancellationToken = default)
        {
            var result = await _repository.IsLockedAsync(requirement.PostId, cancellationToken);
            if (result.IsSuccess && !result.Value)
            {
                return Result.Succeeded;
            }

            return Result.Error(Error.None);
        }
    }
}