#pragma warning disable CS8618
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Shelves.GetShelfByIdWithBooksPaginated;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Shelf;

public class ShelfModel : LoggedPageModel<ShelfModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public ShelfModel(ILogger<ShelfModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string ShelfId { get; set; }

    public Libellus.Domain.Entities.Shelf Shelf { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public string CoverLinkBase { get; set; }

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

    public async Task<IActionResult> OnGetAsync(string gid, string shid, int size, int location)
    {
        GroupId = gid;
        ShelfId = shid;

        var shelfId = await ShelfExistsAsync(shid);
        if (shelfId.IsError)
        {
            return NotFound();
        }

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetShelfByIdWithBooksPaginatedQuery(shelfId.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var shelf = queryResult.Value;
        var url = Url.Page(
            "/Group/Shelf/Shelf",
            pageHandler: null,
            values: new { gid = gid, shid = shid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(shelf!, url!);
        Shelf = shelf.PaginatedItem;

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }
}