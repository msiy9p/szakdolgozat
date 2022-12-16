#pragma warning disable CS8618

using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models.DTOs;
using Libellus.Application.Queries.Books.GetBookById;
using Libellus.Application.Queries.Formats.GetAllFormats;
using Libellus.Application.Queries.Languages.GetAllLanguages;
using Libellus.Application.Queries.Publishers.GetAllPublishers;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Entities;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.BookEditions.DeleteBookEditionCoverImageById;
using Libellus.Application.Commands.BookEditions.UpdateBookEditionById;
using Libellus.Application.Commands.BookEditions.UpdateBookEditionCoverImageById;
using Libellus.Application.Enums;
using Libellus.Domain.Utilities;
using Libellus.Application.Queries.BookEditions.GetBookEditionById;

namespace LibellusWeb.Pages.Group.BookEdition;

public class EditBookEditionModel : LoggedPageModel<EditBookEditionModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public EditBookEditionModel(ILogger<EditBookEditionModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }
    public string BookId { get; set; }
    public string BookEditionId { get; set; }

    public string CoverLinkBase { get; set; }

    public Libellus.Domain.Entities.Book Book { get; set; }
    public Libellus.Domain.Entities.BookEdition BookEdition { get; set; }

    public List<(Format, bool)> Formats { get; set; } = new();
    public List<(Language, bool)> Languages { get; set; } = new();
    public List<(Publisher, bool)> Publishers { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    [BindProperty] public string? InputFormat { get; set; } = string.Empty;
    [BindProperty] public string? InputLanguage { get; set; } = string.Empty;
    [BindProperty] public string? InputPublisher { get; set; } = string.Empty;
    [BindProperty] public string? InputIsbn { get; set; } = string.Empty;
    [BindProperty] public string? InputPublishedOn { get; set; } = string.Empty;
    [BindProperty] public int? InputPageCount { get; set; } = null;
    [BindProperty] public int? InputWordCount { get; set; } = null;

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

        [Required]
        [Display(Name = "Translation?")]
        public bool IsTranslation { get; set; }
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

    private async Task<Result<BookEditionId>> BookEditionExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var bookFriendlyIdId = BookEditionFriendlyId.Convert(friendlyId);
        if (!bookFriendlyIdId.HasValue)
        {
            return DomainErrors.BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(bookFriendlyIdId.Value, cancellationToken);
    }

    private async Task GetFormats(CancellationToken cancellationToken = default)
    {
        var query = new GetAllFormatsQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            foreach (var format in result.Value)
            {
                if (BookEdition.Format is null)
                {
                    Formats.Add(new ValueTuple<Format, bool>(format, false));
                    continue;
                }

                if (BookEdition.Format.Id == format.Id)
                {
                    Formats.Add(new ValueTuple<Format, bool>(format, true));
                }
                else
                {
                    Formats.Add(new ValueTuple<Format, bool>(format, false));
                }
            }
        }
    }

    private async Task GetLanguages(CancellationToken cancellationToken = default)
    {
        var query = new GetAllLanguagesQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            foreach (var language in result.Value)
            {
                if (BookEdition.Language is null)
                {
                    Languages.Add(new ValueTuple<Language, bool>(language, false));
                    continue;
                }

                if (BookEdition.Language.Id == language.Id)
                {
                    Languages.Add(new ValueTuple<Language, bool>(language, true));
                }
                else
                {
                    Languages.Add(new ValueTuple<Language, bool>(language, false));
                }
            }
        }
    }

    private async Task GetPublishers(CancellationToken cancellationToken = default)
    {
        var query = new GetAllPublishersQuery(SortOrder.Ascending);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            foreach (var publisher in result.Value)
            {
                if (BookEdition.Publisher is null)
                {
                    Publishers.Add(new ValueTuple<Publisher, bool>(publisher, false));
                    continue;
                }

                if (BookEdition.Publisher.Id == publisher.Id)
                {
                    Publishers.Add(new ValueTuple<Publisher, bool>(publisher, true));
                }
                else
                {
                    Publishers.Add(new ValueTuple<Publisher, bool>(publisher, false));
                }
            }
        }
    }

    private async Task GetFillableOptions(CancellationToken cancellationToken = default)
    {
        await GetFormats(cancellationToken);
        await GetLanguages(cancellationToken);
        await GetPublishers(cancellationToken);
    }

    private async Task<IActionResult?> LoadData(BookEditionId bookEditionId,
        CancellationToken cancellationToken = default)
    {
        var bookEditionQuery = new GetBookEditionByIdQuery(bookEditionId);
        var bookEditionQueryResult = await _sender.Send(bookEditionQuery, cancellationToken);
        if (bookEditionQueryResult.IsError)
        {
            return NotFound();
        }

        BookEdition = bookEditionQueryResult.Value;

        var bookQuery = new GetBookByIdQuery(BookEdition.BookId);
        var bookQueryResult = await _sender.Send(bookQuery, cancellationToken);
        if (bookQueryResult.IsError)
        {
            return NotFound();
        }

        Book = bookQueryResult.Value;

        await GetFillableOptions(cancellationToken);

        CoverLinkBase = CreateCoverImageUrlBase();

        return null;
    }

    public async Task<IActionResult> OnGetAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        var groupExists = await GroupExistsAsync(gid);
        if (groupExists.IsError)
        {
            return NotFound();
        }

        var bookEditionExists = await BookEditionExistsAsync(beid);
        if (bookEditionExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookEditionExists.Value);
        if (result is not null)
        {
            return result;
        }

        Input = new InputModel()
        {
            Title = BookEdition.Title.Value,
            Description = BookEdition.Description is null ? string.Empty : BookEdition.Description.Value,
            IsTranslation = BookEdition.IsTranslation,
        };

        InputPageCount = BookEdition.PageCount?.Value;
        InputWordCount = BookEdition.WordCount?.Value;
        InputPublishedOn = BookEdition.PublishedOn?.ToString();
        InputIsbn = BookEdition.Isbn?.ToString();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var groupExists = await GroupExistsAsync(gid);
        if (groupExists.IsError)
        {
            return NotFound();
        }

        var bookEditionExists = await BookEditionExistsAsync(beid);
        if (bookEditionExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookEditionExists.Value);
        if (result is not null)
        {
            return result;
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

        FormatDto? format = null;
        if (!string.IsNullOrWhiteSpace(InputFormat))
        {
            var split = InputFormat.Split('-', 2);

            if (split[0].Length == 1 && (split[0][0] == 'T' || split[0][0] == 'F'))
            {
                var isDigital = split[0][0] switch
                {
                    'T' => true,
                    'F' => false,
                    _ => false
                };

                var temp = ShortName.Create(split[1]);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                format = new FormatDto(temp.Value, isDigital, null);
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

                    format = new FormatDto(temp.Value, true, FormatId.Convert(guid));
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

                    format = new FormatDto(temp.Value, true, null);
                }
            }
        }

        LanguageDto? language = null;
        if (!string.IsNullOrWhiteSpace(InputLanguage))
        {
            var split = InputLanguage.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = ShortName.Create(InputLanguage);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                language = new LanguageDto(temp.Value, null);
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = ShortName.Create(split[0]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    language = new LanguageDto(temp.Value, LanguageId.Convert(guid));
                }
                else
                {
                    var temp = ShortName.Create(InputLanguage);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    language = new LanguageDto(temp.Value, null);
                }
            }
        }

        PublisherDto? publisher = null;
        if (!string.IsNullOrWhiteSpace(InputPublisher))
        {
            var split = InputPublisher.Split('-', 2);

            if (split.Length == 1)
            {
                var temp = ShortName.Create(InputPublisher);
                if (temp.IsError)
                {
                    foreach (var error in temp.Errors)
                    {
                        ModelState.AddModelError("", error.Message);
                    }

                    return Page();
                }

                publisher = new PublisherDto(temp.Value, null);
            }
            else
            {
                if (Guid.TryParseExact(split[0], "N", out var guid))
                {
                    var temp = ShortName.Create(split[0]);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    publisher = new PublisherDto(temp.Value, PublisherId.Convert(guid));
                }
                else
                {
                    var temp = ShortName.Create(InputPublisher);
                    if (temp.IsError)
                    {
                        foreach (var error in temp.Errors)
                        {
                            ModelState.AddModelError("", error.Message);
                        }

                        return Page();
                    }

                    publisher = new PublisherDto(temp.Value, null);
                }
            }
        }

        PartialDate? publishedOn = null;
        if (!string.IsNullOrWhiteSpace(InputPublishedOn))
        {
            publishedOn = new PartialDate(InputPublishedOn);
        }

        PageCount? pageCount = null;
        if (InputPageCount.HasValue)
        {
            pageCount = PageCount.Convert(InputPageCount.Value);
        }

        WordCount? wordCount = null;
        if (InputWordCount.HasValue)
        {
            wordCount = WordCount.Convert(InputWordCount.Value);
        }

        Isbn? isbn = null;
        if (!string.IsNullOrWhiteSpace(InputIsbn))
        {
            isbn = Isbn.Convert(InputIsbn);
        }

        var updateCommand = new UpdateBookEditionByIdCommand(
            bookEditionExists.Value,
            title.Value,
            description,
            format,
            language,
            publisher,
            publishedOn,
            Input.IsTranslation,
            pageCount,
            wordCount,
            isbn);

        var editCommandResult = await _sender.Send(updateCommand);
        if (editCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit book edition.");

            return Page();
        }

        return RedirectToPage("/Group/BookEdition/BookEdition", new
        {
            gid = GroupId,
            beid = BookEditionId,
        });
    }

    public async Task<IActionResult> OnPostCoverChangeAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        var bookEditionExists = await BookEditionExistsAsync(beid);
        if (bookEditionExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookEditionExists.Value);
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

        var command = new UpdateBookEditionCoverImageByIdCommand(bookEditionExists.Value, imageCommandResult.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not change cover image.");

            return Page();
        }

        return RedirectToPage("/Group/BookEdition/BookEdition", new { gid = GroupId, beid = BookEditionId });
    }

    public async Task<IActionResult> OnPostCoverDeleteAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        var bookEditionExists = await BookEditionExistsAsync(beid);
        if (bookEditionExists.IsError)
        {
            return NotFound();
        }

        var result = await LoadData(bookEditionExists.Value);
        if (result is not null)
        {
            return result;
        }

        var deleteCommand = new DeleteBookEditionCoverImageByIdCommand(bookEditionExists.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete cover image.");

            return Page();
        }

        return RedirectToPage("/Group/BookEdition/BookEdition", new { gid = GroupId, beid = BookEditionId });
    }
}