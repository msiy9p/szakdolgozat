using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ICommentReadOnlyRepository : IReadOnlyRepository<Comment, CommentId>
{
    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default);

    Task<Result<UserId?>> GetCreatorIdAsync(CommentId id,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Comment>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Result<UserId?> GetCreatorId(CommentId id);
}

public interface ICommentRepository : ICommentReadOnlyRepository, IRepository<Comment, CommentId>
{
}