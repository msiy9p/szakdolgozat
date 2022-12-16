#pragma warning disable CS8618
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Series.GetSeriesByIdWithBooksPaginated;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Series;

public class SeriesModel : LoggedPageModel<SeriesModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public SeriesModel(ILogger<SeriesModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string SeriesId { get; set; }

    public Libellus.Domain.Entities.Series Series { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public string CoverLinkBase { get; set; }

    private async Task<Result<SeriesId>> SeriesExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var shelfFriendlyId = SeriesFriendlyId.Convert(friendlyId);
        if (!shelfFriendlyId.HasValue)
        {
            return DomainErrors.SeriesErrors.SeriesNotFound.ToErrorResult<SeriesId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(shelfFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string seid, int size, int location)
    {
        GroupId = gid;
        SeriesId = seid;

        var seriesId = await SeriesExistsAsync(seid);
        if (seriesId.IsError)
        {
            return NotFound();
        }

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetSeriesByIdWithBooksPaginatedQuery(seriesId.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var shelf = queryResult.Value;
        var url = Url.Page(
            "/Group/Series/Series",
            pageHandler: null,
            values: new { gid = gid, seid = seid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(shelf!, url!);
        Series = shelf.PaginatedItem;

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }
}