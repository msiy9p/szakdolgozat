using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IFriendlyIdLookupRepository
{
    Result<AuthorId> Lookup(AuthorFriendlyId friendlyId);

    Result<BookEditionId> Lookup(BookEditionFriendlyId friendlyId);

    Result<BookId> Lookup(BookFriendlyId friendlyId);

    Result<CommentId> Lookup(CommentFriendlyId friendlyId);

    Result<GroupId> Lookup(GroupFriendlyId friendlyId);

    Result<PostId> Lookup(PostFriendlyId friendlyId);

    Result<ReadingId> Lookup(ReadingFriendlyId friendlyId);

    Result<SeriesId> Lookup(SeriesFriendlyId friendlyId);

    Result<ShelfId> Lookup(ShelfFriendlyId friendlyId);

    Task<Result<AuthorId>> LookupAsync(AuthorFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<BookEditionId>> LookupAsync(BookEditionFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<BookId>> LookupAsync(BookFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<CommentId>> LookupAsync(CommentFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<GroupId>> LookupAsync(GroupFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<PostId>> LookupAsync(PostFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<ReadingId>> LookupAsync(ReadingFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<SeriesId>> LookupAsync(SeriesFriendlyId friendlyId, CancellationToken cancellationToken = default);

    Task<Result<ShelfId>> LookupAsync(ShelfFriendlyId friendlyId, CancellationToken cancellationToken = default);
}