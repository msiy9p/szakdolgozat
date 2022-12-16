#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Posts.LockPostById;
using Libellus.Application.Commands.Posts.UnlockPostById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Queries.Posts.GetPostById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post;

public class LockPostModel : LoggedPageModel<LockPostModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public LockPostModel(ILogger<LockPostModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository,
        IHtmlSanitizer htmlSanitizer) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _htmlSanitizer = htmlSanitizer;
    }

    public string GroupId { get; set; }

    public string PostId { get; set; }

    public Libellus.Domain.Entities.Post Post { get; set; }

    public string LinkBase { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Text")]
        [StringLength(CommentText.MaxLength)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; } = string.Empty;
    }

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

    public async Task<IActionResult> OnGetAsync(string gid, string pid)
    {
        GroupId = gid;
        PostId = pid;

        var postId = await PostExistsAsync(pid);
        if (postId.IsError)
        {
            return NotFound();
        }

        var query = new GetPostByIdQuery(postId.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Post = queryResult.Value;
        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string pid)
    {
        GroupId = gid;
        PostId = pid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var text = CommentText.Create(Input.Text);
        if (text.IsError)
        {
            foreach (var error in text.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        var postId = await PostExistsAsync(PostId);
        if (postId.IsError)
        {
            return NotFound();
        }

        var command = new LockPostByIdCommand(postId.Value, text.Value!);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not lock post.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments", new { gid = GroupId, pid = PostId });
    }

    public async Task<IActionResult> OnPostUnlockAsync(string gid, string pid)
    {
        GroupId = gid;
        PostId = pid;

        var postId = await PostExistsAsync(PostId);
        if (postId.IsError)
        {
            return NotFound();
        }

        var command = new UnlockPostByIdCommand(postId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not unlock post.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments", new { gid = GroupId, pid = PostId });
    }
}