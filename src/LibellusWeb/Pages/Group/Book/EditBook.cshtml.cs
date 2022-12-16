#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Books.DeleteBookById;
using Libellus.Application.Commands.Books.DeleteBookCoverImageById;
using Libellus.Application.Commands.Books.UpdateBookById;
using Libellus.Application.Commands.Books.UpdateBookCoverImageById;
using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Commands.Shelves.AddBookToShelfById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models.DTOs;
using Libellus.Application.Queries.Authors.GetAllAuthors;
using Libellus.Application.Queries.Books.GetBookById;
using Libellus.Application.Queries.Genres.GetAllGenres;
using Libellus.Application.Queries.LiteratureForms.GetAllLiteratureForms;
using Libellus.Application.Queries.Series.GetAllSeries;
using Libellus.Application.Queries.Shelves.GetAllShelvesByBookId;
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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibellusWeb.Pages.Group.Book;

public class EditBookModel : LoggedPageModel<EditBookModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public EditBookModel(ILogger<EditBookModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }
    public string BookId { get; set; }

    public Libellus.Domain.Entities.Book Book { get; set; }

    public List<(LiteratureForm, bool)> LiteratureForms { get; set; } = new();
    public List<(Genre, bool)> Genres { get; set; } = new();

    public List<(Libellus.Domain.Entities.Author, bool)> Authors { get; set; } = new();
    public List<(Tag, bool)> Tags { get; set; } = new();
    public List<(WarningTag, bool)> WarningTags { get; set; } = new();
    public List<(Libellus.Domain.Entities.Series, bool)> Series { get; set; } = new();
    public List<Libellus.Domain.Entities.Shelf> Shelves { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    [BindProperty] public List<string> InputAuthor { get; set; } = new();
    [BindProperty] public List<string> InputTag { get; set; } = new();
    [BindProperty] public List<string> InputWarningTag { get; set; } = new();
    [BindProperty] public List<string> InputGenre { get; set; } = new();
    [BindProperty] public string? InputLiteratureForm { get; set; } = string.Empty;
    [BindProperty] public string? InputSeriesTitle { get; set; } = string.Empty;
    [BindProperty] public decimal? NumberInSeries { get; set; } = null;

    [BindProperty] public string InputShelfId { get; set; } = string.Empty;

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

    private async Task<Result<BookId>> BookExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var bookFriendlyIdId = BookFriendlyId.Convert(friendlyId);
        if (!bookFriendlyIdId.HasValue)
        {
            return DomainErrors.BookErrors.BookNotFound.ToErrorResult<BookId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(bookFriendlyIdId.Value, cancellationToken);
    }

    private async Task<Result<ShelfId>> ShelfExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var shelfFriendlyId = ShelfFriendlyId.Convert(friendlyId);
        if (!shelfFriendlyId.HasValue)
        {
            return DomainErrors.ShelfErrors.ShelfNotFound.ToErrorResult<ShelfId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(shelfFriendlyId.Value, cancellationToken);
    }

    private async Task GetAuthors(CancellationToken cancellationToken = default)
    {
        var query = new GetAllAuthorsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            var authorNames = Book.Authors
                .Select(x => x.Name)
                .ToList();

            foreach (var author in result.Value)
            {
                if (authorNames.Contains(author.Name))
                {
                    Authors.Add(new ValueTuple<Libellus.Domain.Entities.Author, bool>(author, true));
                }
                else
                {
                    Authors.Add(new ValueTuple<Libellus.Domain.Entities.Author, bool>(author, false));
                }
            }
        }
    }

    private async Task GetTags(CancellationToken cancellationToken = default)
    {
        var query = new GetAllTagsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            var tagNames = Book.Tags
                .Select(x => x.Name)
                .ToList();

            foreach (var tag in result.Value)
            {
                if (tagNames.Contains(tag.Name))
                {
                    Tags.Add(new ValueTuple<Tag, bool>(tag, true));
                }
                else
                {
                    Tags.Add(new ValueTuple<Tag, bool>(tag, false));
                }
            }
        }
    }

    private async Task GetWarningTags(CancellationToken cancellationToken = default)
    {
        var query = new GetAllWarningTagsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            var warningTagNames = Book.WarningTags
                .Select(x => x.Name)
                .ToList();

            foreach (var warningTag in result.Value)
            {
                if (warningTagNames.Contains(warningTag.Name))
                {
                    WarningTags.Add(new ValueTuple<WarningTag, bool>(warningTag, true));
                }
                else
                {
                    WarningTags.Add(new ValueTuple<WarningTag, bool>(warningTag, false));
                }
            }
        }
    }

    private async Task GetGenres(CancellationToken cancellationToken = default)
    {
        var query = new GetAllGenresQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            var genreNames = Book.Genres
                .Select(x => x.Name)
                .ToList();

            foreach (var genre in result.Value)
            {
                if (genreNames.Contains(genre.Name))
                {
                    Genres.Add(new ValueTuple<Genre, bool>(genre, true));
                }
                else
                {
                    Genres.Add(new ValueTuple<Genre, bool>(genre, false));
                }
            }
        }
    }

    private async Task GetLiteratureForms(CancellationToken cancellationToken = default)
    {
        var query = new GetAllLiteratureFormsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            foreach (var literatureForm in result.Value)
            {
                if (Book.LiteratureForm is null)
                {
                    LiteratureForms.Add(new ValueTuple<LiteratureForm, bool>(literatureForm, false));
                    continue;
                }

                if (Book.LiteratureForm.Id == literatureForm.Id)
                {
                    LiteratureForms.Add(new ValueTuple<LiteratureForm, bool>(literatureForm, true));
                }
                else
                {
                    LiteratureForms.Add(new ValueTuple<LiteratureForm, bool>(literatureForm, false));
                }
            }
        }
    }

    private async Task GetSeries(CancellationToken cancellationToken = default)
    {
        var query = new GetAllSeriesQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            foreach (var series in result.Value)
            {
                if (Book.Series is null)
                {
                    Series.Add(new ValueTuple<Libellus.Domain.Entities.Series, bool>(series, false));
                    continue;
                }

                if (Book.Series.Id == series.Id)
                {
                    Series.Add(new ValueTuple<Libellus.Domain.Entities.Series, bool>(series, true));
                }
                else
                {
                    Series.Add(new ValueTuple<Libellus.Domain.Entities.Series, bool>(series, false));
                }
            }
        }
    }

    private async Task GetShelves(BookId bookId, CancellationToken cancellationToken = default)
    {
        var query = new GetAllShelvesByBookIdQuery(bookId, Containing: false, SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            Shelves.AddRange(result.Value);
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

    private async Task<IActionResult?> LoadData(BookId bookId, CancellationToken cancellationToken = default)
    {
        var query = new GetBookByIdQuery(bookId);
        var queryResult = await _sender.Send(query, cancellationToken);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Book = queryResult.Value;
        await GetFillableOptions(cancellationToken);
        await GetShelves(bookId, cancellationToken);

        return null;
    }

    public async Task<IActionResult> OnGetAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var groupExists = await GroupExistsAsync(gid);
        if (groupExists.IsError)
        {
            return NotFound();
        }

        var bookExists = await BookExistsAsync(bid);
        if (bookExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookExists.Value);
        if (result is not null)
        {
            return result;
        }

        Input = new InputModel()
        {
            Title = Book.Title.Value,
            Description = Book.Description is null ? string.Empty : Book.Description.Value,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var bookExists = await BookExistsAsync(bid);
        if (bookExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookExists.Value);
        if (result is not null)
        {
            return result;
        }

        if (!ModelState.IsValid)
        {
            if (ModelState.ErrorCount == 1 && ModelState.ContainsKey(nameof(InputShelfId)))
            {
            }
            else
            {
                return Page();
            }
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
                    var temp = Title.Create(split[1]);
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

        var updateCommand = new UpdateBookByIdCommand(
            bookExists.Value,
            title.Value,
            description,
            literatureForm,
            series,
            authors,
            genres,
            tags,
            warningTags);

        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not update book.");

            return Page();
        }

        return RedirectToPage("/Group/Book/Book", new { gid = GroupId, bid = BookId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var bookExists = await BookExistsAsync(bid);
        if (bookExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookExists.Value);
        if (result is not null)
        {
            return result;
        }

        var command = new DeleteBookByIdCommand(bookExists.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete book.");

            return Page();
        }

        return RedirectToPage("/Group/Book/Books", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostCoverChangeAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var bookExists = await BookExistsAsync(bid);
        if (bookExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookExists.Value);
        if (result is not null)
        {
            return result;
        }

        if (Upload is null)
        {
            ModelState.AddModelError("", "Invalid file.");

            return Page();
        }

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

        var command = new UpdateBookCoverImageByIdCommand(bookExists.Value, imageCommandResult.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not change cover image.");

            return Page();
        }

        return RedirectToPage("/Group/Book/Book", new { gid = GroupId, bid = BookId });
    }

    public async Task<IActionResult> OnPostCoverDeleteAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var bookExists = await BookExistsAsync(bid);
        if (bookExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookExists.Value);
        if (result is not null)
        {
            return result;
        }

        var deleteCommand = new DeleteBookCoverImageByIdCommand(bookExists.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete cover image.");

            return Page();
        }

        return RedirectToPage("/Group/Book/Book", new { gid = GroupId, bid = BookId });
    }

    public async Task<IActionResult> OnPostAddShelfAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var bookExists = await BookExistsAsync(bid);
        if (bookExists.IsError)
        {
            return NotFound();
        }

        var shelfExists = await ShelfExistsAsync(InputShelfId);
        if (shelfExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookExists.Value);
        if (result is not null)
        {
            return result;
        }

        if (!ModelState.IsValid)
        {
            ModelState.TryGetValue(nameof(InputShelfId), out var value);

            if (value is null || value.ValidationState == ModelValidationState.Invalid)
            {
                return Page();
            }
        }

        var command = new AddBookToShelfByIdCommand(shelfExists.Value, bookExists.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not add to shelf.");

            return Page();
        }

        return RedirectToPage("/Group/Book/EditBook", new { gid = GroupId, bid = BookId });
    }
}