using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IGroupReadOnlyRepository
{
    UserId CurrentUserId { get; }

    Task<Result<bool>> ExistsAsync(GroupId id, CancellationToken cancellationToken = default);

    Task<Result<Group>> GetByIdAsync(GroupId id, CancellationToken cancellationToken = default);

    Task<Result<GroupNameVm>> GetNameVmByIdAsync(GroupId id, CancellationToken cancellationToken = default);

    Task<Result<Group>> GetByIdWithPostsAsync(GroupId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<Group>>> GetByIdWithPostsAsync(GroupId id, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Group>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Group>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<Group>>> GetAllMemberAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Group>>>> GetAllMemberAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<GroupNameVm>>> GetAllMemberNamesAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<Group>>> FindByNameAsync(Name name, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Group>>>> FindByNameAsync(Name name, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);

    Task<Result<ICollection<Group>>> SearchAsync(SearchTerm searchTerm, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);

    Task<Result<PaginationDetail<ICollection<Group>>>> SearchAsync(SearchTerm searchTerm, PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default);
}

public interface IGroupRepository : IGroupReadOnlyRepository
{
    Task<Result> AddIfNotExistsAsync(Group entity, CancellationToken cancellationToken = default);

    Task<Result> AddWithDefaultIfNotExistsAsync(Group entity, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(Group entity, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(GroupId id, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Group entity, CancellationToken cancellationToken = default);
}