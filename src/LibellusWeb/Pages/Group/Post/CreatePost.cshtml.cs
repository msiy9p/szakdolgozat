#pragma warning disable CS8618
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Domain.ValueObjects;
using Libellus.Application.Commands.Posts.CreatePost;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Labels.GetAllLabels;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Net;

namespace LibellusWeb.Pages.Group.Post;

public class CreatePostModel : LoggedPageModel<CreatePostModel>
{
    private readonly ISender _sender;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public CreatePostModel(ILogger<CreatePostModel> logger,
        ISender sender, IHtmlSanitizer htmlSanitizer) : base(logger)
    {
        _sender = sender;
        _htmlSanitizer = htmlSanitizer;
    }

    public string GroupId { get; set; }

    public List<string> Labels { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(Libellus.Domain.ValueObjects.Title.MaxLength)]
        public string Title { get; set; }

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

        [Display(Name = "Label")]
        [StringLength(ShortName.MaxLength)]
        public string Label { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string gid)
    {
        GroupId = gid;

        var query = new GetAllLabelsQuery(SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            ModelState.AddModelError("", "Could not get labels.");

            return Page();
        }

        Labels.Add("No label");
        foreach (var label in queryResult.Value)
        {
            Labels.Add(label.Name);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid)
    {
        GroupId = gid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var title = Title.Create(Input.Title);
        if (title.IsError)
        {
            foreach (var error in title.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

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

        var command = new CreatePostCommand(title.Value!, text.Value!, Input.IsMemberOnly,
            Input.IsSpoiler, label);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create post.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Comment/Comments",
            new { gid = GroupId, pid = commandResult.Value!.PostFriendlyId.Value });
    }
}