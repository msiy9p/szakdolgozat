#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.DeleteUserById;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class DeletePersonalDataModel : LoggedPageModel<DeletePersonalDataModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public DeletePersonalDataModel(ILogger<DeletePersonalDataModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var deleteCommand = new DeleteUserByIdCommand(result.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            foreach (var error in deleteCommandResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return Page();
        }

        return Redirect("~/");
    }
}