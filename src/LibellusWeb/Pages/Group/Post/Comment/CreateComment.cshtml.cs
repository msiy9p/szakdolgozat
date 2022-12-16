#pragma warning disable CS8618
using Libellus.Domain.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Comments.CreateComment;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Queries.Posts.GetPostById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Utilities;
using System.Net;

namespace LibellusWeb.Pages.Group.Post.Comment;

public class CreateCommentModel : LoggedPageModel<CreateCommentModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public CreateCommentModel(ILogger<CreateCommentModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository, IHtmlSanitizer htmlSanitizer) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _htmlSanitizer = htmlSanitizer;
    }

    public string GroupId { get; set; }

    public string PostId { get; set; }

    public string LinkBase { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Text")]
        [StringLength(CommentText.MaxLength)]
        [DataType(DataType.Html)]
        public string Text { get; set; }
    }

    public Libellus.Domain.Entities.Post Post { get; set; }

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

        var postQuery = new GetPostByIdQuery(postId.Value);
        var post = await _sender.Send(postQuery);
        if (post.IsError)
        {
            return NotFound();
        }

        Post = post.Value;

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

        var postId = await PostExistsAsync(pid);
        if (postId.IsError)
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

        var command = new CreateCommentCommand(postId.Value, text.Value!, RepliedToCommentId: null);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create comment.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments",
            new { gid = GroupId, pid = PostId });
    }
}