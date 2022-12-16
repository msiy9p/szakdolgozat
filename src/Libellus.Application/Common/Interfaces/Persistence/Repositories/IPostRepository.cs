using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IPostReadOnlyRepository : IReadOnlyRepository<Post, PostId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<int>> GetCommentCountAsync(PostId id, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsLockedAsync(PostId id, CancellationToken cancellationToken = default);

    Task<Result<UserId?>> GetCreatorIdAsync(PostId id,
        CancellationToken cancellationToken = default);

    Task<Result<Post>> GetByIdWithCommentsAsync(PostId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<Post>>> GetByIdWithCommentsAsync(PostId id, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Post>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Post>>> GetAllAsync(ShortName labelName,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Post>>>> GetAllAsync(ShortName labelName, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Post>>> FindByTitleAsync(Title title, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Post>>>> FindByTitleAsync(Title title, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Post>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Post>>>> SearchAsync(SearchTerm searchTerm, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(PostId id);
}

public interface IPostRepository : IPostReadOnlyRepository, IRepository<Post, PostId>
{
    Task<Result> DeleteByFriendlyIdAsync(PostFriendlyId friendlyId, CancellationToken cancellationToken = default);
}