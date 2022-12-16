#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Groups.CreateGroup;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group;

public class CreateGroupModel : LoggedPageModel<CreateGroupModel>
{
    private readonly ISender _sender;

    public CreateGroupModel(ILogger<CreateGroupModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(Libellus.Domain.ValueObjects.Name.MaxLength)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(DescriptionText.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Private?")]
        public bool IsPrivate { get; set; }

        [Required]
        [Display(Name = "Create With default values?")]
        public bool CreateWithDefaultValues { get; set; }
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var name = Name.Create(Input.Name);
        if (name.IsError)
        {
            foreach (var error in name.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        DescriptionText? description = null;
        if (!string.IsNullOrWhiteSpace(Input.Description))
        {
            var descResult = DescriptionText.Create(Input.Description);
            if (descResult.IsError)
            {
                foreach (var error in descResult.Errors)
                {
                    ModelState.AddModelError("", error.Message);
                }

                return Page();
            }

            description = descResult.Value;
        }

        var command = new CreateGroupCommand(name.Value, description, Input.IsPrivate, Input.CreateWithDefaultValues);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create group.");

            return Page();
        }

        return RedirectToPage("/Group/Post/Posts",
            new
            {
                gid = commandResult.Value.GroupFriendlyId.Value
            });
    }
}