#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.Net;
using Libellus.Application.Commands.Comments.DeleteCommentById;
using Libellus.Application.Commands.Comments.UpdateCommentById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Queries.Comments.GetCommentById;
using Libellus.Application.Queries.Posts.GetPostById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post.Comment;

public class EditCommentModel : LoggedPageModel<EditCommentModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public EditCommentModel(ILogger<EditCommentModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository, IHtmlSanitizer htmlSanitizer) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _htmlSanitizer = htmlSanitizer;
    }

    public string GroupId { get; set; }

    public string PostId { get; set; }

    public string CommentId { get; set; }

    public Libellus.Domain.Entities.Post Post { get; set; }

    public string LinkBase { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public CreateCommentModel.InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Text")]
        [StringLength(CommentText.MaxLength)]
        [DataType(DataType.Html)]
        public string Text { get; set; }
    }

    private async Task<Result<PostId>> PostExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var friendlyIdId = PostFriendlyId.Convert(friendlyId);
        if (!friendlyIdId.HasValue)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<PostId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(friendlyIdId.Value, cancellationToken);
    }

    private async Task<Result<CommentId>> CommentExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var friendlyIdId = CommentFriendlyId.Convert(friendlyId);
        if (!friendlyIdId.HasValue)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<CommentId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(friendlyIdId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string pid, string cid)
    {
        var postId = await PostExistsAsync(pid);
        if (postId.IsError)
        {
            return NotFound();
        }

        var commentId = await CommentExistsAsync(cid);
        if (commentId.IsError)
        {
            return NotFound();
        }

        GroupId = gid;
        PostId = pid;
        CommentId = cid;


        var postQuery = new GetPostByIdQuery(postId.Value);
        var post = await _sender.Send(postQuery);
        if (post.IsError)
        {
            return NotFound();
        }

        var commentQuery = new GetCommentByIdQuery(commentId.Value);
        var comment = await _sender.Send(commentQuery);
        if (comment.IsError)
        {
            return NotFound();
        }

        Post = post.Value;

        LinkBase = CreateProfilePictureUrlBase();

        Input = new CreateCommentModel.InputModel() { Text = comment.Value!.Text };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string pid, string cid)
    {
        GroupId = gid;
        PostId = pid;
        CommentId = cid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var commentId = await CommentExistsAsync(CommentId);
        if (commentId.IsError)
        {
            return NotFound();
        }

        var text = CommentText.Create(_htmlSanitizer.Sanitize(WebUtility.HtmlDecode(Input.Text)));
        if (text.IsError)
        {
            foreach (var error in text.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        var command = new UpdateCommentByIdCommand(commentId.Value, text.Value!);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit comment.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments",
            new { gid = GroupId, pid = PostId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string pid, string cid)
    {
        GroupId = gid;
        PostId = pid;
        CommentId = cid;

        //if (!ModelState.IsValid)
        //{
        //    return Page();
        //}

        var postId = await PostExistsAsync(PostId);
        if (postId.IsError)
        {
            return NotFound();
        }

        var commentId = await CommentExistsAsync(CommentId);
        if (commentId.IsError)
        {
            return NotFound();
        }

        var command = new DeleteCommentByIdCommand(commentId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete comment.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments",
            new { gid = GroupId, pid = PostId });
    }
}