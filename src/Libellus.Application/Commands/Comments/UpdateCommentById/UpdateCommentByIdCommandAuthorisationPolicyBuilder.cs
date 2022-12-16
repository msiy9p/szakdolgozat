using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Comments.UpdateCommentById;

public sealed class
    UpdateCommentByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        UpdateCommentByIdCommand>
{
    private readonly ICommentReadOnlyRepository _repository;
    private readonly ICurrentPostService _currentPostService;

    public UpdateCommentByIdCommandAuthorisationPolicyBuilder(ICommentReadOnlyRepository repository,
        ICurrentPostService currentPostService)
    {
        _repository = repository;
        _currentPostService = currentPostService;
    }

    public override void BuildPolicy(UpdateCommentByIdCommand instance)
    {
        var exists = _repository.GetCreatorId(instance.CommentId);
        if (exists.IsError)
        {
            UseRequirement(new SameCreatorRequirement(null));
        }
        else
        {
            UseRequirement(new SameCreatorRequirement(exists.Value));
        }

        var postId = _currentPostService.CurrentPostId;
        if (!postId.HasValue)
        {
            // Throw exception?
            UseRequirement(FailAuthorisationRequirement.Instance);
        }
        else
        {
            UseRequirement(new PostNotLockedRequirement(postId.Value));
        }
    }
}