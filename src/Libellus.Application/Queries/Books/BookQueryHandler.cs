using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Books.GetAllBooks;
using Libellus.Application.Queries.Books.GetAllBooksByAuthorId;
using Libellus.Application.Queries.Books.GetAllBooksByAuthorIdPaginated;
using Libellus.Application.Queries.Books.GetAllBooksPaginated;
using Libellus.Application.Queries.Books.GetAllCompactBooksByShelfId;
using Libellus.Application.Queries.Books.GetBookById;
using Libellus.Application.Queries.Books.GetBookByIdWithBookEdition;
using Libellus.Application.Queries.Books.GetBookByTitle;
using Libellus.Application.Queries.Books.GetBookByTitlePaginated;
using Libellus.Application.Queries.Books.SearchBooks;
using Libellus.Application.Queries.Books.SearchBooksPaginated;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Queries.Books;

public sealed class BookQueryHandler :
    IQueryHandler<GetBookByIdQuery, Book>,
    IQueryHandler<GetBookByIdWithBookEditionQuery, Book>,
    IQueryHandler<GetAllBooksQuery, ICollection<Book>>,
    IQueryHandler<GetAllBooksPaginatedQuery, PaginationDetail<ICollection<Book>>>,
    IQueryHandler<GetBookByTitleQuery, ICollection<Book>>,
    IQueryHandler<GetBookByTitlePaginatedQuery, PaginationDetail<ICollection<Book>>>,
    IQueryHandler<SearchBooksQuery, ICollection<Book>>,
    IQueryHandler<SearchBooksPaginatedQuery, PaginationDetail<ICollection<Book>>>,
    IQueryHandler<GetAllBooksByAuthorIdQuery, ICollection<Book>>,
    IQueryHandler<GetAllBooksByAuthorIdPaginatedQuery, PaginationDetail<ICollection<Book>>>,
    IQueryHandler<GetAllCompactBooksByShelfIdQuery, ICollection<BookCompactVm>>
{
    private readonly IBookReadOnlyRepository _repository;

    public BookQueryHandler(IBookReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Book>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.BookId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Book>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Book>>>> Handle(GetAllBooksPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Book>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Book>>> Handle(GetBookByTitleQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByTitleAsync(request.Title, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Book>>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<Book>> Handle(GetBookByIdWithBookEditionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithBookEditionsAsync(request.BookId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Book>>> Handle(GetAllBooksByAuthorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllByAuthorIdAsync(request.AuthorId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Book>>>> Handle(GetAllBooksByAuthorIdPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Book>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllByAuthorIdAsync(request.AuthorId, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Book>>>> Handle(GetBookByTitlePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Book>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByTitleAsync(request.Title, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Book>>>> Handle(SearchBooksPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Book>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<BookCompactVm>>> Handle(GetAllCompactBooksByShelfIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllCompactVmByShelfIdAsync(request.ShelfId, request.SortOrder,
            cancellationToken: cancellationToken);
    }
}