#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.Net;
using Libellus.Application.Commands.Posts.DeletePostById;
using Libellus.Application.Commands.Posts.UpdatePostById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Labels.GetAllLabels;
using Libellus.Application.Queries.Posts.GetPostById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post;

public class EditPostModel : LoggedPageModel<EditPostModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public EditPostModel(ILogger<EditPostModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository,
        IHtmlSanitizer htmlSanitizer) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _htmlSanitizer = htmlSanitizer;
    }

    public string GroupId { get; set; }

    public string PostId { get; set; }

    public string Title { get; set; }

    public bool IsLocked { get; set; }

    public List<string> Labels { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Text")]
        [StringLength(CommentText.MaxLength)]
        [DataType(DataType.Html)]
        public string Text { get; set; }

        [Required]
        [Display(Name = "Spoiler?")]
        public bool IsSpoiler { get; set; }

        [Required]
        [Display(Name = "Member only?")]
        public bool IsMemberOnly { get; set; }

        [Required]
        [Display(Name = "Label")]
        [StringLength(ShortName.MaxLength)]
        public string Label { get; set; }
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

        var labelQuery = new GetAllLabelsQuery(SortOrder.Ascending);
        var labelQueryResult = await _sender.Send(labelQuery);
        if (labelQueryResult.IsSuccess)
        {
            if (queryResult.Value.Label is null)
            {
                Labels.Add("No label");

                foreach (var label in labelQueryResult.Value!)
                {
                    Labels.Add(label.Name);
                }
            }
            else
            {
                Labels.Add(queryResult.Value.Label.Name);
                Labels.Add("No label");

                foreach (var label in labelQueryResult.Value!.Where(x => x.Name != queryResult.Value.Label.Name))
                {
                    Labels.Add(label.Name);
                }
            }
        }
        else
        {
            Labels.Add("No label");
        }

        Input = new InputModel()
        {
            Text = queryResult.Value!.Text,
            IsSpoiler = queryResult.Value!.IsSpoiler,
            IsMemberOnly = queryResult.Value!.IsMemberOnly,
            Label = queryResult.Value!.Label?.Name ?? "No label",
        };

        Title = queryResult.Value!.Title;
        IsLocked = queryResult.Value!.IsLocked;

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

        var text = CommentText.Create(_htmlSanitizer.Sanitize(WebUtility.HtmlDecode(Input.Text)));
        if (text.IsError)
        {
            foreach (var error in text.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        ShortName? label = null;
        if (!string.IsNullOrWhiteSpace(Input.Label) &&
            Input.Label.ToNormalizedUpperInvariant() != "no label".ToNormalizedUpperInvariant())
        {
            var temp = ShortName.Create(Input.Label);
            if (temp.IsError)
            {
                foreach (var error in temp.Errors)
                {
                    ModelState.AddModelError("", error.Message);
                }

                return Page();
            }

            label = temp.Value;
        }

        var postId = await PostExistsAsync(PostId);
        if (postId.IsError)
        {
            return NotFound();
        }

        var command = new UpdatePostByIdCommand(postId.Value, text.Value!,
            Input.IsMemberOnly, Input.IsSpoiler, label);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit post.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments", new { gid = GroupId, pid = PostId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string pid)
    {
        GroupId = gid;
        PostId = pid;

        //if (!ModelState.IsValid)
        //{
        //    return Page();
        //}

        var postId = await PostExistsAsync(PostId);
        if (postId.IsError)
        {
            return NotFound();
        }

        var command = new DeletePostByIdCommand(postId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete post.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Posts", new { gid = GroupId });
    }
}