#pragma warning disable CS8618
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Posts.GetPostByIdWithCommentsPaginated;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using LibellusWeb.Models;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post.Comment;

public class CommentsModel : LoggedPageModel<CommentsModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public CommentsModel(ILogger<CommentsModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public List<PageNavigation> PageNavigations { get; set; }

    public Libellus.Domain.Entities.Post Post { get; set; }

    public bool IsCreator { get; set; } = false;
    public UserId UserId { get; set; }

    public string GroupId { get; set; }
    public string PostId { get; set; }
    public string LinkBase { get; set; }

    private async Task<Result<PostId>> PostExistsAsync(string postFriendlyId,
        CancellationToken cancellationToken = default)
    {
        var postFriendlyIdId = PostFriendlyId.Convert(postFriendlyId);
        if (!postFriendlyIdId.HasValue)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<PostId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(postFriendlyIdId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string pid, int size, int location)
    {
        GroupId = gid;
        PostId = pid;

        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        UserId = result.Value;

        var postId = await PostExistsAsync(pid);
        if (postId.IsError)
        {
            return NotFound();
        }

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetPostByIdWithCommentsPaginatedQuery(postId.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var post = queryResult.Value;
        var url = Url.Page(
            $"/Group/Post/Comment/Comments",
            pageHandler: null,
            values: new { gid = gid, pid = pid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(post!, url!);
        Post = post!.PaginatedItem;

        if (Post.CreatorId is not null && Post.CreatorId.Value == result.Value)
        {
            IsCreator = true;
        }

        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }
}