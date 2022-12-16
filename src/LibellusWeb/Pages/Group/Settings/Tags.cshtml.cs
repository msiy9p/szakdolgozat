#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Formats.CreateFormat;
using Libellus.Application.Commands.Formats.DeleteFormatById;
using Libellus.Application.Commands.Formats.UpdateFormatById;
using Libellus.Application.Commands.Genres.CreateGenre;
using Libellus.Application.Commands.Genres.DeleteGenreById;
using Libellus.Application.Commands.Genres.UpdateGenreById;
using Libellus.Application.Commands.Labels.CreateLabel;
using Libellus.Application.Commands.Labels.DeleteLabelById;
using Libellus.Application.Commands.Labels.UpdateLabelById;
using Libellus.Application.Commands.Languages.CreateLanguage;
using Libellus.Application.Commands.Languages.DeleteLanguageById;
using Libellus.Application.Commands.Languages.UpdateLanguageById;
using Libellus.Application.Commands.LiteratureForms.CreateLiteratureForm;
using Libellus.Application.Commands.LiteratureForms.DeleteLiteratureFormById;
using Libellus.Application.Commands.LiteratureForms.UpdateLiteratureFormById;
using Libellus.Application.Commands.Publishers.CreatePublisher;
using Libellus.Application.Commands.Publishers.DeletePublisherById;
using Libellus.Application.Commands.Publishers.UpdatePublisherById;
using Libellus.Application.Commands.Tags.CreateTag;
using Libellus.Application.Commands.Tags.DeleteTagById;
using Libellus.Application.Commands.Tags.UpdateTagById;
using Libellus.Application.Commands.WarningTags.CreateWarningTag;
using Libellus.Application.Commands.WarningTags.DeleteWarningTagById;
using Libellus.Application.Commands.WarningTags.UpdateWarningTagById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Formats.GetAllFormats;
using Libellus.Application.Queries.Genres.GetAllGenres;
using Libellus.Application.Queries.Labels.GetAllLabels;
using Libellus.Application.Queries.Languages.GetAllLanguages;
using Libellus.Application.Queries.LiteratureForms.GetAllLiteratureForms;
using Libellus.Application.Queries.Publishers.GetAllPublishers;
using Libellus.Application.Queries.Tags.GetAllTags;
using Libellus.Application.Queries.WarningTags.GetAllWarningTags;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;

namespace LibellusWeb.Pages.Group.Settings;

public class TagsModel : LoggedPageModel<TagsModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public TagsModel(ILogger<TagsModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public List<LiteratureForm> LiteratureForms { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();

    public List<Tag> Tags { get; set; } = new();
    public List<WarningTag> WarningTags { get; set; } = new();

    public List<Format> Formats { get; set; } = new();
    public List<Language> Languages { get; set; } = new();
    public List<Publisher> Publishers { get; set; } = new();

    public List<Label> Labels { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Display(Name = "Input")]
        [StringLength(ShortName.MaxLength)]
        [DataType(DataType.Text)]
        public string InputValue { get; set; } = string.Empty;

        [DataType(DataType.Text)] public string? InputId { get; set; } = string.Empty;
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

    private async Task GetTags(CancellationToken cancellationToken = default)
    {
        var query = new GetAllTagsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Tags.AddRange(result.Value);
        }
    }

    private async Task GetWarningTags(CancellationToken cancellationToken = default)
    {
        var query = new GetAllWarningTagsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            WarningTags.AddRange(result.Value);
        }
    }

    private async Task GetGenres(CancellationToken cancellationToken = default)
    {
        var query = new GetAllGenresQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Genres.AddRange(result.Value);
        }
    }

    private async Task GetLiteratureForms(CancellationToken cancellationToken = default)
    {
        var query = new GetAllLiteratureFormsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            LiteratureForms.AddRange(result.Value);
        }
    }

    private async Task GetFormats(CancellationToken cancellationToken = default)
    {
        var query = new GetAllFormatsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Formats.AddRange(result.Value);
        }
    }

    private async Task GetLanguages(CancellationToken cancellationToken = default)
    {
        var query = new GetAllLanguagesQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Languages.AddRange(result.Value);
        }
    }

    private async Task GetPublishers(CancellationToken cancellationToken = default)
    {
        var query = new GetAllPublishersQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Publishers.AddRange(result.Value);
        }
    }

    private async Task GetLabels(CancellationToken cancellationToken = default)
    {
        var query = new GetAllLabelsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Labels.AddRange(result.Value);
        }
    }

    private async Task GetFillableOptions(CancellationToken cancellationToken = default)
    {
        await GetTags(cancellationToken);
        await GetWarningTags(cancellationToken);
        await GetGenres(cancellationToken);
        await GetLiteratureForms(cancellationToken);

        await GetFormats(cancellationToken);
        await GetPublishers(cancellationToken);
        await GetLanguages(cancellationToken);

        await GetLabels(cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        await GetFillableOptions();

        return Page();
    }

    public async Task<IActionResult> OnPostEditGenreAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = GenreId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateGenreByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteGenreAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = GenreId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteGenreByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddGenreAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        // TODO: implement Fiction selector

        var createCommand = new CreateGenreCommand(name.Value, true);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditLitFormAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = LiteratureFormId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateLiteratureFormByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteLitFormAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = LiteratureFormId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteLiteratureFormByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddLiteratureFormAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        // TODO: implement ScoreMultiplier selector

        var createCommand = new CreateLiteratureFormCommand(name.Value, new ScoreMultiplier());
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditTagAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = TagId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateTagByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteTagAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = TagId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteTagByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddTagAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        var createCommand = new CreateTagCommand(name.Value);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditWarningTagAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = WarningTagId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateWarningTagByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteWarningTagAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = WarningTagId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteWarningTagByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddWarningTagAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        var createCommand = new CreateWarningTagCommand(name.Value);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditFormatAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = FormatId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateFormatByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteFormatAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = FormatId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteFormatByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddFormatAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        // TODO: implement IsDigital selector

        var createCommand = new CreateFormatCommand(name.Value, true);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditPublisherAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = PublisherId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdatePublisherByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeletePublisherAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = PublisherId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeletePublisherByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddPublisherAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        var createCommand = new CreatePublisherCommand(name.Value);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditLanguageAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = LanguageId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateLanguageByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteLanguageAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = LanguageId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteLanguageByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddLanguageAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        var createCommand = new CreateLanguageCommand(name.Value);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostEditLabelAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var id = LabelId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        var updateCommand = new UpdateLabelByIdCommand(id.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteLabelAsync(string gid)
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

        if (!Guid.TryParse(Input.InputId, out var guid))
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var id = LabelId.Convert(guid);
        if (!id.HasValue)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        var deleteCommand = new DeleteLabelByIdCommand(id.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostAddLabelAsync(string gid)
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

        var name = ShortName.Create(Input.InputValue);
        if (name.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        var createCommand = new CreateLabelCommand(name.Value);
        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Tags", new { gid = GroupId });
    }
}