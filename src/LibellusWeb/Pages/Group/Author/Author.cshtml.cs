#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Authors.GetAuthorById;
using Libellus.Application.Queries.Books.GetAllBooksByAuthorIdPaginated;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Author;

public class AuthorModel : LoggedPageModel<AuthorModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public AuthorModel(ILogger<AuthorModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string AuthorId { get; set; }

    public Libellus.Domain.Entities.Author Author { get; set; }

    public List<Libellus.Domain.Entities.Book> Books { get; set; } = new();

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public string CoverLinkBase { get; set; }

    private async Task<Result<AuthorId>> AuthorExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var authorFriendlyId = AuthorFriendlyId.Convert(friendlyId);
        if (!authorFriendlyId.HasValue)
        {
            return DomainErrors.AuthorErrors.AuthorNotFound.ToErrorResult<AuthorId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(authorFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string aid, int size, int location)
    {
        GroupId = gid;
        AuthorId = aid;

        var authorId = await AuthorExistsAsync(aid);
        if (authorId.IsError)
        {
            return NotFound();
        }

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var authorQuery = new GetAuthorByIdQuery(authorId.Value);
        var authorQueryResult = await _sender.Send(authorQuery);
        if (authorQueryResult.IsError)
        {
            return NotFound();
        }

        var booksQuery = new GetAllBooksByAuthorIdPaginatedQuery(authorId.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var booksQueryResult = await _sender.Send(booksQuery);
        if (booksQueryResult.IsError)
        {
            return NotFound();
        }

        var shelf = booksQueryResult.Value;
        var url = Url.Page(
            "/Group/Author/Author",
            pageHandler: null,
            values: new { gid = gid, aid = aid, size = "25", location = "1" },
            protocol: Request.Scheme);

        Author = authorQueryResult.Value;
        PageNavigations = PageNavigation.CreateNavigations(shelf!, url!);
        Books.AddRange(shelf.PaginatedItem);

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }
}