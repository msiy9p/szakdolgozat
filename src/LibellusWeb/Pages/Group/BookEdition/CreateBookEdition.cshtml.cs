#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.BookEditions.CreateBookEdition;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Utilities;
using Libellus.Application.Queries.Publishers.GetAllPublishers;
using Libellus.Application.Queries.Languages.GetAllLanguages;
using Libellus.Application.Queries.Formats.GetAllFormats;
using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Models.DTOs;
using Libellus.Application.Queries.Books.GetBookById;
using Libellus.Domain.Common.Types;
using Libellus.Domain.ValueObjects;

namespace LibellusWeb.Pages.Group.BookEdition;

public class CreateBookEditionModel : LoggedPageModel<CreateBookEditionModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public CreateBookEditionModel(ILogger<CreateBookEditionModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }
    public string BookId { get; set; }

    public string CoverLinkBase { get; set; }

    public Libellus.Domain.Entities.Book Book { get; set; }

    public List<Format> Formats { get; set; } = new();
    public List<Language> Languages { get; set; } = new();
    public List<Publisher> Publishers { get; set; } = new();

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

    private async Task GetFillableOptions(CancellationToken cancellationToken = default)
    {
        await GetFormats(cancellationToken);
        await GetLanguages(cancellationToken);
        await GetPublishers(cancellationToken);
    }

    private async Task<IActionResult?> LoadData(BookId bookId, CancellationToken cancellationToken = default)
    {
        var query = new GetBookByIdQuery(bookId);
        var queryResult = await _sender.Send(query, cancellationToken);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        await GetFillableOptions(cancellationToken);

        Book = queryResult.Value;

        CoverLinkBase = CreateCoverImageUrlBase();

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

        return result ?? Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string bid)
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

        var createCommand = new CreateBookEditionCommand(
            bookExists.Value,
            title.Value,
            description,
            format,
            language,
            publisher,
            publishedOn,
            Input.IsTranslation,
            pageCount,
            wordCount,
            isbn,
            metaDataContainer);

        var createCommandResult = await _sender.Send(createCommand);
        if (createCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create book edition.");

            return Page();
        }

        return RedirectToPage("/Group/BookEdition/BookEdition", new
        {
            gid = GroupId,
            beid = createCommandResult.Value.BookEditionFriendlyId.Value,
        });
    }
}