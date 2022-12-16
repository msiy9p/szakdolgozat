#nullable disable

using System.Text.Json;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class DownloadPersonalDataModel : LoggedPageModel<DownloadPersonalDataModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public DownloadPersonalDataModel(ILogger<DownloadPersonalDataModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public IActionResult OnGet()
    {
        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        // Only include personal data for download
        var personalData = new Dictionary<string, string>();
        //var personalDataProps = typeof(User).GetProperties().Where(
        //    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        //foreach (var p in personalDataProps)
        //{
        //    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        //}

        //var logins = await _userManager.GetLoginsAsync(user);
        //foreach (var l in logins)
        //{
        //    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        //}

        //personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

        Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}