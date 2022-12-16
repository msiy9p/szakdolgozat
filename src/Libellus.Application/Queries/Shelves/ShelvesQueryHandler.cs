using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Shelves.GetAllShelves;
using Libellus.Application.Queries.Shelves.GetAllShelvesByBookId;
using Libellus.Application.Queries.Shelves.GetAllShelvesPaginated;
using Libellus.Application.Queries.Shelves.GetShelfById;
using Libellus.Application.Queries.Shelves.GetShelfByIdWithBooks;
using Libellus.Application.Queries.Shelves.GetShelfByIdWithBooksPaginated;
using Libellus.Application.Queries.Shelves.GetShelfByName;
using Libellus.Application.Queries.Shelves.SearchShelves;
using Libellus.Application.Queries.Shelves.SearchShelvesPaginated;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Shelves;

public sealed class ShelvesQueryHandler :
    IQueryHandler<GetShelfByIdQuery, Shelf>,
    IQueryHandler<GetShelfByIdWithBooksQuery, Shelf>,
    IQueryHandler<GetShelfByIdWithBooksPaginatedQuery, PaginationDetail<Shelf>>,
    IQueryHandler<GetAllShelvesQuery, ICollection<Shelf>>,
    IQueryHandler<GetAllShelvesPaginatedQuery, PaginationDetail<ICollection<Shelf>>>,
    IQueryHandler<GetShelfByNameQuery, Shelf>,
    IQueryHandler<SearchShelvesQuery, ICollection<Shelf>>,
    IQueryHandler<SearchShelvesPaginatedQuery, PaginationDetail<ICollection<Shelf>>>,
    IQueryHandler<GetAllShelvesByBookIdQuery, ICollection<Shelf>>
{
    private readonly IShelfReadOnlyRepository _repository;

    public ShelvesQueryHandler(IShelfReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Shelf>> Handle(GetShelfByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.ShelfId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Shelf>>> Handle(GetAllShelvesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<Shelf>> Handle(GetShelfByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Shelf>>> Handle(SearchShelvesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<Shelf>> Handle(GetShelfByIdWithBooksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithBooksAsync(request.ShelfId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<Shelf>>> Handle(GetShelfByIdWithBooksPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<Shelf>>.Error(paginationResult.Errors);
        }

        return await _repository.GetByIdWithBooksAsync(request.ShelfId, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Shelf>>>> Handle(GetAllShelvesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Shelf>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Shelf>>>> Handle(SearchShelvesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Shelf>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value,
            request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Shelf>>> Handle(GetAllShelvesByBookIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetShelvesByBookIdAsync(request.BookId, request.Containing, request.SortOrder,
            cancellationToken);
    }
}