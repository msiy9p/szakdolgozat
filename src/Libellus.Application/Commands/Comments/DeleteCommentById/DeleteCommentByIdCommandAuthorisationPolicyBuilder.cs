using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Comments.DeleteCommentById;

public sealed class
    DeleteCommentByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteCommentByIdCommand>
{
    private readonly ICommentReadOnlyRepository _repository;
    private readonly ICurrentPostService _currentPostService;

    public DeleteCommentByIdCommandAuthorisationPolicyBuilder(ICommentReadOnlyRepository repository,
        ICurrentPostService currentPostService)
    {
        _repository = repository;
        _currentPostService = currentPostService;
    }

    public override void BuildPolicy(DeleteCommentByIdCommand instance)
    {
        var exists = _repository.GetCreatorId(instance.CommentId);
        if (exists.IsError)
        {
            UseRequirement(new SameCreatorOrAboveRequirement(null));
        }
        else
        {
            UseRequirement(new SameCreatorOrAboveRequirement(exists.Value));
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