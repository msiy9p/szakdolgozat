using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.BookEditions.GetAllBookEditions;
using Libellus.Application.Queries.BookEditions.GetAllBookEditionsPaginated;
using Libellus.Application.Queries.BookEditions.GetBookEditionById;
using Libellus.Application.Queries.BookEditions.GetBookEditionByIdWithReadings;
using Libellus.Application.Queries.BookEditions.GetBookEditionByIsbn;
using Libellus.Application.Queries.BookEditions.GetBookEditionByIsbnPaginated;
using Libellus.Application.Queries.BookEditions.GetBookEditionByTitle;
using Libellus.Application.Queries.BookEditions.GetBookEditionByTitlePaginated;
using Libellus.Application.Queries.BookEditions.SearchBookEditions;
using Libellus.Application.Queries.BookEditions.SearchBookEditionsPaginated;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.BookEditions;

public sealed class BookEditionQueryHandler :
    IQueryHandler<GetBookEditionByIdQuery, BookEdition>,
    IQueryHandler<GetBookEditionByIdWithReadingsQuery, BookEdition>,
    IQueryHandler<GetAllBookEditionsQuery, ICollection<BookEdition>>,
    IQueryHandler<GetAllBookEditionsPaginatedQuery, PaginationDetail<ICollection<BookEdition>>>,
    IQueryHandler<GetBookEditionByTitleQuery, ICollection<BookEdition>>,
    IQueryHandler<GetBookEditionByTitlePaginatedQuery, PaginationDetail<ICollection<BookEdition>>>,
    IQueryHandler<SearchBookEditionsQuery, ICollection<BookEdition>>,
    IQueryHandler<SearchBookEditionsPaginatedQuery, PaginationDetail<ICollection<BookEdition>>>,
    IQueryHandler<GetBookEditionByIsbnQuery, ICollection<BookEdition>>,
    IQueryHandler<GetBookEditionByIsbnPaginatedQuery, PaginationDetail<ICollection<BookEdition>>>
{
    private readonly IBookEditionReadOnlyRepository _repository;

    public BookEditionQueryHandler(IBookEditionReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BookEdition>> Handle(GetBookEditionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.BookEditionId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<BookEdition>>> Handle(GetAllBookEditionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<BookEdition>>>> Handle(
        GetAllBookEditionsPaginatedQuery request, CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<BookEdition>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<BookEdition>>> Handle(GetBookEditionByTitleQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByTitleAsync(request.Title, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<BookEdition>>> Handle(SearchBookEditionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<BookEdition>> Handle(GetBookEditionByIdWithReadingsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithReadingsAsync(request.BookEditionId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<BookEdition>>> Handle(GetBookEditionByIsbnQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByIsbnAsync(request.Isbn, request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<BookEdition>>>> Handle(
        GetBookEditionByTitlePaginatedQuery request, CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<BookEdition>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByTitleAsync(request.Title, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<BookEdition>>>> Handle(
        SearchBookEditionsPaginatedQuery request, CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<BookEdition>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<BookEdition>>>> Handle(
        GetBookEditionByIsbnPaginatedQuery request, CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<BookEdition>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByIsbnAsync(request.Isbn, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }
}