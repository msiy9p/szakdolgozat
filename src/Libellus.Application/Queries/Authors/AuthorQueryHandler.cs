using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Authors.GetAllAuthors;
using Libellus.Application.Queries.Authors.GetAllAuthorsPaginated;
using Libellus.Application.Queries.Authors.GetAuthorById;
using Libellus.Application.Queries.Authors.GetAuthorByName;
using Libellus.Application.Queries.Authors.GetAuthorByNamePaginated;
using Libellus.Application.Queries.Authors.SearchAuthors;
using Libellus.Application.Queries.Authors.SearchAuthorsPaginated;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Authors;

public sealed class AuthorQueryHandler :
    IQueryHandler<GetAuthorByIdQuery, Author>,
    IQueryHandler<GetAllAuthorsQuery, ICollection<Author>>,
    IQueryHandler<GetAllAuthorsPaginatedQuery, PaginationDetail<ICollection<Author>>>,
    IQueryHandler<GetAuthorByNameQuery, ICollection<Author>>,
    IQueryHandler<GetAuthorByNamePaginatedQuery, PaginationDetail<ICollection<Author>>>,
    IQueryHandler<SearchAuthorsQuery, ICollection<Author>>,
    IQueryHandler<SearchAuthorsPaginatedQuery, PaginationDetail<ICollection<Author>>>
{
    private readonly IAuthorReadOnlyRepository _repository;

    public AuthorQueryHandler(IAuthorReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Author>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.AuthorId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Author>>> Handle(GetAllAuthorsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Author>>>> Handle(GetAllAuthorsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Author>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Author>>> Handle(GetAuthorByNameQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Author>>> Handle(SearchAuthorsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Author>>>> Handle(GetAuthorByNamePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Author>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByNameAsync(request.Name, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Author>>>> Handle(SearchAuthorsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Author>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }
}