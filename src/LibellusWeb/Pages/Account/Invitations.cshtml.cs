using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Invitations.AcceptInvitation;
using Libellus.Application.Commands.Invitations.DeclineInvitation;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Invitations.GetInvitationVmsByInvitee;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class InvitationsModel : LoggedPageModel<InvitationsModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public InvitationsModel(ILogger<InvitationsModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string LinkBase { get; set; }

    public List<InvitationUserVm> Invitations { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string InvitationId { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var query = new GetInvitationVmsByInviteeQuery(result.Value, InvitationStatus.Pending, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Invitations.AddRange(queryResult.Value);

        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostAcceptAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!Guid.TryParse(Input.InvitationId, out var guidId))
        {
            ModelState.AddModelError("", "Cannot find invitation.");
            return Page();
        }

        var invitationId = InvitationId.Convert(guidId);
        if (!invitationId.HasValue)
        {
            ModelState.AddModelError("", "Cannot find invitation.");
            return Page();
        }

        var command = new AcceptInvitationCommand(invitationId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            foreach (var error in commandResult.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        return RedirectToPage("/Account/Invitations");
    }

    public async Task<IActionResult> OnPostDeclineAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!Guid.TryParse(Input.InvitationId, out var guidId))
        {
            ModelState.AddModelError("", "Cannot find invitation.");
            return Page();
        }

        var invitationId = InvitationId.Convert(guidId);
        if (!invitationId.HasValue)
        {
            ModelState.AddModelError("", "Cannot find invitation.");
            return Page();
        }

        var command = new DeclineInvitationCommand(invitationId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            foreach (var error in commandResult.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        return RedirectToPage("/Account/Invitations");
    }
}