using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Groups.GetAllGroups;
using Libellus.Application.Queries.Groups.GetAllGroupsPaginated;
using Libellus.Application.Queries.Groups.GetAllMemberGroups;
using Libellus.Application.Queries.Groups.GetAllMemberGroupsPaginated;
using Libellus.Application.Queries.Groups.GetGroupById;
using Libellus.Application.Queries.Groups.GetGroupByIdWithPosts;
using Libellus.Application.Queries.Groups.GetGroupByIdWithPostsPaginated;
using Libellus.Application.Queries.Groups.GetGroupByName;
using Libellus.Application.Queries.Groups.GetGroupByNamePaginated;
using Libellus.Application.Queries.Groups.GetGroupNameById;
using Libellus.Application.Queries.Groups.GetMemberGroups;
using Libellus.Application.Queries.Groups.SearchGroups;
using Libellus.Application.Queries.Groups.SearchGroupsPaginated;
using Libellus.Application.ViewModels;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Groups;

public sealed class GroupQueryHandler :
    IQueryHandler<GetGroupByIdQuery, Group>,
    IQueryHandler<GetGroupNameByIdQuery, GroupNameVm>,
    IQueryHandler<GetGroupByIdWithPostsQuery, Group>,
    IQueryHandler<GetGroupByIdWithPostsPaginatedQuery, PaginationDetail<Group>>,
    IQueryHandler<GetAllGroupsQuery, ICollection<Group>>,
    IQueryHandler<GetAllGroupsPaginatedQuery, PaginationDetail<ICollection<Group>>>,
    IQueryHandler<GetAllMemberGroupsQuery, ICollection<Group>>,
    IQueryHandler<GetAllMemberGroupsPaginatedQuery, PaginationDetail<ICollection<Group>>>,
    IQueryHandler<GetGroupByNameQuery, ICollection<Group>>,
    IQueryHandler<GetGroupByNamePaginatedQuery, PaginationDetail<ICollection<Group>>>,
    IQueryHandler<GetMemberGroupsQuery, ICollection<GroupNameVm>>,
    IQueryHandler<SearchGroupsQuery, ICollection<Group>>,
    IQueryHandler<SearchGroupsPaginatedQuery, PaginationDetail<ICollection<Group>>>
{
    private readonly IGroupReadOnlyRepository _repository;

    public GroupQueryHandler(IGroupReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Group>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Group>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Group>>>> Handle(GetAllGroupsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Group>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Group>>> Handle(GetGroupByNameQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Group>>> Handle(SearchGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<Group>> Handle(GetGroupByIdWithPostsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithPostsAsync(request.GroupId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<Group>>> Handle(GetGroupByIdWithPostsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<Group>>.Error(paginationResult.Errors);
        }

        return await _repository.GetByIdWithPostsAsync(request.GroupId, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Group>>>> Handle(SearchGroupsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Group>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Group>>>> Handle(GetGroupByNamePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Group>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByNameAsync(request.Name, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<GroupNameVm>>> Handle(GetMemberGroupsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllMemberNamesAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Group>>> Handle(GetAllMemberGroupsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllMemberAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Group>>>> Handle(GetAllMemberGroupsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Group>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllMemberAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<GroupNameVm>> Handle(GetGroupNameByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetNameVmByIdAsync(request.GroupId, cancellationToken: cancellationToken);
    }
}