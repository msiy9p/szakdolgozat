using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class PersonalDataModel : LoggedPageModel<PersonalDataModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public PersonalDataModel(ILogger<PersonalDataModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public IActionResult OnGet()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        return Page();
    }
}