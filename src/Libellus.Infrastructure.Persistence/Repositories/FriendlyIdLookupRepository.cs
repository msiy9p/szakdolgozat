using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class FriendlyIdLookupRepository : BaseRepository<FriendlyIdLookupRepository>,
    IFriendlyIdLookupRepository
{
    public FriendlyIdLookupRepository(ApplicationContext context, ILogger<FriendlyIdLookupRepository> logger) : base(
        context, logger)
    {
    }

    public Result<AuthorId> Lookup(AuthorFriendlyId friendlyId)
    {
        var found = Context.Authors
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.AuthorErrors.AuthorNotFound.ToErrorResult<AuthorId>();
        }

        return found.ToResult();
    }

    public Result<BookEditionId> Lookup(BookEditionFriendlyId friendlyId)
    {
        var found = Context.BookEditions
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionId>();
        }

        return found.ToResult();
    }

    public Result<BookId> Lookup(BookFriendlyId friendlyId)
    {
        var found = Context.Books
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.BookErrors.BookNotFound.ToErrorResult<BookId>();
        }

        return found.ToResult();
    }

    public Result<CommentId> Lookup(CommentFriendlyId friendlyId)
    {
        var found = Context.Comments
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.CommentErrors.CommentNotFound.ToErrorResult<CommentId>();
        }

        return found.ToResult();
    }

    public Result<GroupId> Lookup(GroupFriendlyId friendlyId)
    {
        var found = Context.Groups
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult<GroupId>();
        }

        return found.ToResult();
    }

    public Result<PostId> Lookup(PostFriendlyId friendlyId)
    {
        var found = Context.Posts
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<PostId>();
        }

        return found.ToResult();
    }

    public Result<ReadingId> Lookup(ReadingFriendlyId friendlyId)
    {
        var found = Context.Readings
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.ReadingErrors.ReadingNotFound.ToErrorResult<ReadingId>();
        }

        return found.ToResult();
    }

    public Result<SeriesId> Lookup(SeriesFriendlyId friendlyId)
    {
        var found = Context.Series
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.SeriesErrors.SeriesNotFound.ToErrorResult<SeriesId>();
        }

        return found.ToResult();
    }

    public Result<ShelfId> Lookup(ShelfFriendlyId friendlyId)
    {
        var found = Context.Shelves
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (found == default)
        {
            return DomainErrors.ShelfErrors.ShelfNotFound.ToErrorResult<ShelfId>();
        }

        return found.ToResult();
    }

    public async Task<Result<AuthorId>> LookupAsync(AuthorFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Authors
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.AuthorErrors.AuthorNotFound.ToErrorResult<AuthorId>();
        }

        return found.ToResult();
    }

    public async Task<Result<BookEditionId>> LookupAsync(BookEditionFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.BookEditions
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionId>();
        }

        return found.ToResult();
    }

    public async Task<Result<BookId>> LookupAsync(BookFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Books
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.BookErrors.BookNotFound.ToErrorResult<BookId>();
        }

        return found.ToResult();
    }

    public async Task<Result<CommentId>> LookupAsync(CommentFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Comments
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.CommentErrors.CommentNotFound.ToErrorResult<CommentId>();
        }

        return found.ToResult();
    }

    public async Task<Result<GroupId>> LookupAsync(GroupFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult<GroupId>();
        }

        return found.ToResult();
    }

    public async Task<Result<PostId>> LookupAsync(PostFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Posts
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<PostId>();
        }

        return found.ToResult();
    }

    public async Task<Result<ReadingId>> LookupAsync(ReadingFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Readings
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.ReadingErrors.ReadingNotFound.ToErrorResult<ReadingId>();
        }

        return found.ToResult();
    }

    public async Task<Result<SeriesId>> LookupAsync(SeriesFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Series
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.SeriesErrors.SeriesNotFound.ToErrorResult<SeriesId>();
        }

        return found.ToResult();
    }

    public async Task<Result<ShelfId>> LookupAsync(ShelfFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Shelves
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return DomainErrors.ShelfErrors.ShelfNotFound.ToErrorResult<ShelfId>();
        }

        return found.ToResult();
    }
}