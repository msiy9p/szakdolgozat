#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.GroupMemberships.GetGroupMembershipById;
using Libellus.Application.Queries.Users.GetUserPicturedVmById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.GroupMemberships.DemoteModerator;
using Libellus.Application.Commands.GroupMemberships.DemoteOwner;
using Libellus.Application.Commands.GroupMemberships.KickOutMember;
using Libellus.Application.Commands.GroupMemberships.LeaveCurrentGroup;
using Libellus.Application.Commands.GroupMemberships.PromoteModerator;
using Libellus.Application.Commands.GroupMemberships.PromoteUser;

namespace LibellusWeb.Pages.Group.Settings;

public class MembersModel : LoggedPageModel<MembersModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public MembersModel(ILogger<MembersModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string LinkBase { get; set; }

    public List<UserPicturedVm> Owners { get; set; } = new();
    public List<UserPicturedVm> Moderators { get; set; } = new();
    public List<UserPicturedVm> Members { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Display(Name = "InputId")]
        [DataType(DataType.Text)]
        public string InputId { get; set; } = string.Empty;
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

        var query = new GetGroupMembershipByIdQuery(exists.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        foreach (var membershipItem in queryResult.Value.GetItems())
        {
            var userQuery = new GetUserPicturedVmByIdQuery(membershipItem.UserId);
            var userQueryResult = await _sender.Send(userQuery);
            if (userQueryResult.IsError)
            {
                continue;
            }

            switch (membershipItem.GroupRole)
            {
                case GroupRole.Member:
                    Members.Add(userQueryResult.Value);
                    break;
                case GroupRole.Moderator:
                    Moderators.Add(userQueryResult.Value);
                    break;
                case GroupRole.Owner:
                    Owners.Add(userQueryResult.Value);
                    break;
                default:
                    break;
            }
        }

        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostOwnerDemoteAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var id = UserId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var command = new DemoteOwnerCommand(id.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Members", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostModeratorPromoteAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var id = UserId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var command = new PromoteModeratorCommand(id.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Members", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostModeratorDemoteAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var id = UserId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var command = new DemoteModeratorCommand(id.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Members", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostMemberPromoteAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var id = UserId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var command = new PromoteUserCommand(id.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Members", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostMemberKickAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var id = UserId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var command = new KickOutMemberCommand(id.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Members", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostLeaveAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var id = UserId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        var command = LeaveCurrentGroupCommand.Instance;
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not do action.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Members", new { gid = GroupId });
    }
}