#pragma warning disable CS8618
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Users.GetUserIdByName;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Invitations.InviteUser;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Invitations.GetInvitationVmsByGroup;
using Libellus.Application.ViewModels;
using Libellus.Domain.Enums;
using Libellus.Domain.ValueObjects;

namespace LibellusWeb.Pages.Group.Settings;

public class InvitationsModel : LoggedPageModel<InvitationsModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public InvitationsModel(ILogger<InvitationsModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string LinkBase { get; set; }

    public List<InvitationPicturedVm> Invitations { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(UserName.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.Text)]
        public string Username { get; set; } = string.Empty;
    }

    private async Task<Result<GroupId>> GroupExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var groupFriendlyIdId = GroupFriendlyId.Convert(friendlyId);
        if (!groupFriendlyIdId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult<GroupId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(groupFriendlyIdId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        var query = new GetInvitationVmsByGroupQuery(exists.Value, InvitationStatus.Pending, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Invitations.AddRange(queryResult.Value);

        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid)
    {
        GroupId = gid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        var userResult = UserName.Create(Input.Username);
        if (userResult.IsError)
        {
            foreach (var error in userResult.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        var query = new GetUserIdByNameQuery(userResult.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var command = new InviteUserCommand(queryResult.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not invite user.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Invitations");
    }
}