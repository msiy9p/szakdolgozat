#pragma warning disable CS8618
using Libellus.Application.Commands.Shelves.DeleteShelfById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Shelves.RemoveBookFromShelfById;
using Libellus.Application.Commands.Shelves.UpdateShelfById;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Books.GetAllCompactBooksByShelfId;
using Libellus.Application.Queries.Shelves.GetShelfById;
using Libellus.Domain.ViewModels;

namespace LibellusWeb.Pages.Group.Shelf;

public class EditShelfModel : LoggedPageModel<EditShelfModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public EditShelfModel(ILogger<EditShelfModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string ShelfId { get; set; }

    public string ShelfName { get; set; }

    public List<BookCompactVm> Books { get; set; } = new();

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Display(Name = "Description")]
        [StringLength(DescriptionText.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;

        [Required] [Display(Name = "Locked?")] public bool IsLocked { get; set; }
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

    public async Task<IActionResult> OnGetAsync(string gid, string shid)
    {
        GroupId = gid;
        ShelfId = shid;

        var shelfId = await ShelfExistsAsync(shid);
        if (shelfId.IsError)
        {
            return NotFound();
        }

        var query = new GetShelfByIdQuery(shelfId.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var bookQuery = new GetAllCompactBooksByShelfIdQuery(shelfId.Value, SortOrder.Ascending);
        var bookQueryResult = await _sender.Send(bookQuery);
        if (bookQueryResult.IsError)
        {
            return NotFound();
        }

        Books.AddRange(bookQueryResult.Value);
        ShelfName = queryResult.Value.Name.Value;

        Input = new InputModel()
        {
            Description = queryResult.Value.Description?.Value,
            IsLocked = queryResult.Value.IsLocked,
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string shid)
    {
        GroupId = gid;
        ShelfId = shid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var shelfId = await ShelfExistsAsync(shid);
        if (shelfId.IsError)
        {
            return NotFound();
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

        var command = new UpdateShelfByIdCommand(shelfId.Value, description, Input.IsLocked);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit shelf.");

            return Page();
        }

        return RedirectToPage("/Group/Shelf/Shelf", new { gid = GroupId, shid = ShelfId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string shid)
    {
        GroupId = gid;
        ShelfId = shid;

        var shelfId = await ShelfExistsAsync(shid);
        if (shelfId.IsError)
        {
            return NotFound();
        }

        var command = new DeleteShelfByIdCommand(shelfId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete shelf.");

            return Page();
        }

        return RedirectToPage("/Group/Shelf/Shelves", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostDeleteBookAsync(string gid, string shid, string inputBookId)
    {
        GroupId = gid;
        ShelfId = shid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var shelfId = await ShelfExistsAsync(shid);
        if (shelfId.IsError)
        {
            return NotFound();
        }

        var bookId = await BookExistsAsync(inputBookId);
        if (bookId.IsError)
        {
            return NotFound();
        }

        var command = new RemoveBookFromShelfByIdCommand(shelfId.Value, bookId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not remove book from shelf.");

            return Page();
        }

        return RedirectToPage("/Group/Shelf/EditShelf", new { gid = GroupId, shid = ShelfId });
    }
}