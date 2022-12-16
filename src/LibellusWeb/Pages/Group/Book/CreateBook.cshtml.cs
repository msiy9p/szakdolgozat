#pragma warning disable CS8618

using Libellus.Domain.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Books.CreateBook;
using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Authors.GetAllAuthors;
using Libellus.Application.Queries.Genres.GetAllGenres;
using Libellus.Application.Queries.LiteratureForms.GetAllLiteratureForms;
using Libellus.Application.Queries.Series.GetAllSeries;
using Libellus.Application.Queries.Tags.GetAllTags;
using Libellus.Application.Queries.WarningTags.GetAllWarningTags;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Errors;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Application.Models.DTOs;
using Libellus.Domain.Common.Types;

namespace LibellusWeb.Pages.Group.Book;

public class CreateBookModel : LoggedPageModel<CreateBookModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public CreateBookModel(ILogger<CreateBookModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public List<LiteratureForm> LiteratureForms { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();

    public List<Libellus.Domain.Entities.Author> Authors { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
    public List<WarningTag> WarningTags { get; set; } = new();
    public List<Libellus.Domain.Entities.Series> Series { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    [BindProperty] public List<string> InputAuthor { get; set; } = new();
    [BindProperty] public List<string> InputTag { get; set; } = new();
    [BindProperty] public List<string> InputWarningTag { get; set; } = new();
    [BindProperty] public List<string> InputGenre { get; set; } = new();
    [BindProperty] public string? InputLiteratureForm { get; set; } = string.Empty;
    [BindProperty] public string? InputSeriesTitle { get; set; } = string.Empty;
    [BindProperty] public decimal? NumberInSeries { get; set; } = null;

    [BindProperty]
    [DataType(DataType.Upload)]
    public IFormFile? Upload { get; set; } = null;

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(Libellus.Domain.ValueObjects.Title.MaxLength)]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [StringLength(DescriptionText.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;
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

    private async Task GetAuthors(CancellationToken cancellationToken = default)
    {
        var query = new GetAllAuthorsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Authors.AddRange(result.Value);
        }
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

    private async Task GetSeries(CancellationToken cancellationToken = default)
    {
        var query = new GetAllSeriesQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Series.AddRange(result.Value);
        }
    }

    private async Task GetFillableOptions(CancellationToken cancellationToken = default)
    {
        await GetAuthors(cancellationToken);
        await GetTags(cancellationToken);
        await GetWarningTags(cancellationToken);
        await GetGenres(cancellationToken);
        await GetLiteratureForms(cancellationToken);
        await GetSeries(cancellationToken);
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

    public async Task<IActionResult> OnPostAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        await GetFillableOptions();

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

        LiteratureFormDto? literatureForm = null;
        if (!string.IsNullOrWhiteSpace(InputLiteratureForm))
        {
            var split = InputLiteratureForm.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = ShortName.Create(InputLiteratureForm);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                literatureForm = new LiteratureFormDto(temp.Value,
                    new ScoreMultiplier(ScoreMultiplier.ScoreMultiplierDefault), null);
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = ShortName.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    literatureForm = new LiteratureFormDto(temp.Value,
                        new ScoreMultiplier(ScoreMultiplier.ScoreMultiplierDefault), LiteratureFormId.Convert(guid));
                }
                else
                {
                    var temp = ShortName.Create(InputLiteratureForm);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    literatureForm = new LiteratureFormDto(temp.Value,
                        new ScoreMultiplier(ScoreMultiplier.ScoreMultiplierDefault), null);
                }
            }
        }

        SeriesDto? series = null;
        if (!string.IsNullOrWhiteSpace(InputSeriesTitle) &&
            Libellus.Domain.Entities.Book.IsValidNumberInSeries(NumberInSeries))
        {
            var split = InputSeriesTitle.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = Title.Create(InputSeriesTitle);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                series = new SeriesDto(temp.Value, NumberInSeries!.Value, null);
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = Title.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    series = new SeriesDto(temp.Value, NumberInSeries!.Value, SeriesId.Convert(guid));
                }
                else
                {
                    var temp = Title.Create(InputSeriesTitle);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    series = new SeriesDto(temp.Value, NumberInSeries!.Value, null);
                }
            }
        }

        var authors = new List<AuthorDto>(InputAuthor.Count);
        foreach (var item in InputAuthor)
        {
            var split = item.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = Name.Create(item);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                if (authors.All(x => x.Name != temp.Value))
                {
                    authors.Add(new AuthorDto(temp.Value, null));
                }
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = Name.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (authors.All(x => x.Name != temp.Value))
                    {
                        authors.Add(new AuthorDto(temp.Value, AuthorId.Convert(guid)));
                    }
                }
                else
                {
                    var temp = Name.Create(item);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (authors.All(x => x.Name != temp.Value))
                    {
                        authors.Add(new AuthorDto(temp.Value, null));
                    }
                }
            }
        }

        var genres = new List<GenreDto>(InputGenre.Count);
        foreach (var item in InputGenre)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            var split = item.Split('-', 2);

            if (split.Length == 1)
            {
                continue;
            }

            if (split[0].Length == 1)
            {
                bool isFiction;
                if (split[0][0] == 'T')
                {
                    isFiction = true;
                }
                else if (split[0][0] == 'F')
                {
                    isFiction = false;
                }
                else
                {
                    continue;
                }

                var temp = ShortName.Create(split[1]);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                if (genres.All(x => x.ShortName != temp.Value))
                {
                    genres.Add(new GenreDto(temp.Value, isFiction, null));
                }
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = ShortName.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (genres.All(x => x.ShortName != temp.Value))
                    {
                        genres.Add(new GenreDto(temp.Value, true, GenreId.Convert(guid)));
                    }
                }
                else
                {
                    var temp = ShortName.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (genres.All(x => x.ShortName != temp.Value))
                    {
                        genres.Add(new GenreDto(temp.Value, true, null));
                    }
                }
            }
        }

        var tags = new List<TagDto>(InputTag.Count);
        foreach (var item in InputTag)
        {
            var split = item.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = ShortName.Create(item);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                if (tags.All(x => x.Name != temp.Value))
                {
                    tags.Add(new TagDto(temp.Value, null));
                }
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = ShortName.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (tags.All(x => x.Name != temp.Value))
                    {
                        tags.Add(new TagDto(temp.Value, TagId.Convert(guid)));
                    }
                }
                else
                {
                    var temp = ShortName.Create(item);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (tags.All(x => x.Name != temp.Value))
                    {
                        tags.Add(new TagDto(temp.Value, null));
                    }
                }
            }
        }

        var warningTags = new List<WarningTagDto>(InputWarningTag.Count);
        foreach (var item in InputWarningTag)
        {
            var split = item.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = ShortName.Create(item);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                if (warningTags.All(x => x.Name != temp.Value))
                {
                    warningTags.Add(new WarningTagDto(temp.Value, null));
                }
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = ShortName.Create(split[1]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (warningTags.All(x => x.Name != temp.Value))
                    {
                        warningTags.Add(new WarningTagDto(temp.Value, WarningTagId.Convert(guid)));
                    }
                }
                else
                {
                    var temp = ShortName.Create(item);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    if (warningTags.All(x => x.Name != temp.Value))
                    {
                        warningTags.Add(new WarningTagDto(temp.Value, null));
                    }
                }
            }
        }

        CoverImageMetaDataContainer? metaDataContainer = null;
        if (Upload is not null)
        {
            Result<ImageDataOnly> imageData;
            await using (var stream = Upload.OpenReadStream())
            {
                if (stream is MemoryStream ms)
                {
                    imageData = ImageDataOnly.Create(ms.ToArray());
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        imageData = ImageDataOnly.Create(memoryStream.ToArray());
                    }
                }
            }

            if (imageData.IsError)
            {
                ModelState.AddModelError("", "Invalid file.");

                return Page();
            }

            var imageCommand = new CreateCoverImagesCommand(imageData.Value);
            var imageCommandResult = await _sender.Send(imageCommand);
            if (imageCommandResult.IsError)
            {
                ModelState.AddModelError("", "Could not create cover image.");
                return Page();
            }

            metaDataContainer = imageCommandResult.Value;
        }

        var createCommand = new CreateBookCommand(
            title.Value,
            description,
            literatureForm,
            series,
            metaDataContainer,
            authors,
            genres,
            tags,
            warningTags);

        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create book.");

            return Page();
        }

        return RedirectToPage("/Group/Book/Book", new
        {
            gid = GroupId,
            bid = createCommandResult.Value.BookFriendlyId.Value,
        });
    }
}