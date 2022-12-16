using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Series.GetAllSeries;
using Libellus.Application.Queries.Series.GetAllSeriesPaginated;
using Libellus.Application.Queries.Series.GetSeriesById;
using Libellus.Application.Queries.Series.GetSeriesByIdWithBooks;
using Libellus.Application.Queries.Series.GetSeriesByIdWithBooksPaginated;
using Libellus.Application.Queries.Series.GetSeriesByTitle;
using Libellus.Application.Queries.Series.GetSeriesByTitlePaginated;
using Libellus.Application.Queries.Series.SearchSeries;
using Libellus.Application.Queries.Series.SearchSeriesPaginated;
using Libellus.Domain.Models;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series;

public sealed class SeriesQueryHandler :
    IQueryHandler<GetSeriesByIdQuery, DomainSeries>,
    IQueryHandler<GetSeriesByIdWithBooksQuery, DomainSeries>,
    IQueryHandler<GetSeriesByIdWithBooksPaginatedQuery, PaginationDetail<DomainSeries>>,
    IQueryHandler<GetAllSeriesQuery, ICollection<DomainSeries>>,
    IQueryHandler<GetAllSeriesPaginatedQuery, PaginationDetail<ICollection<DomainSeries>>>,
    IQueryHandler<GetSeriesByTitleQuery, ICollection<DomainSeries>>,
    IQueryHandler<GetSeriesByTitlePaginatedQuery, PaginationDetail<ICollection<DomainSeries>>>,
    IQueryHandler<SearchSeriesQuery, ICollection<DomainSeries>>,
    IQueryHandler<SearchSeriesPaginatedQuery, PaginationDetail<ICollection<DomainSeries>>>
{
    private readonly ISeriesReadOnlyRepository _repository;

    public SeriesQueryHandler(ISeriesReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DomainSeries>> Handle(GetSeriesByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.SeriesId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<DomainSeries>>> Handle(GetAllSeriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<DomainSeries>>> Handle(GetSeriesByTitleQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByTitleAsync(request.Title, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<DomainSeries>>> Handle(SearchSeriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<DomainSeries>> Handle(GetSeriesByIdWithBooksQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithBooksAsync(request.SeriesId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<DomainSeries>>> Handle(GetSeriesByIdWithBooksPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<DomainSeries>>.Error(paginationResult.Errors);
        }

        return await _repository.GetByIdWithBooksAsync(request.SeriesId, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<DomainSeries>>>> Handle(
        GetSeriesByTitlePaginatedQuery request, CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<DomainSeries>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByTitleAsync(request.Title, paginationResult.Value,
            request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<DomainSeries>>>> Handle(SearchSeriesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<DomainSeries>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value,
            request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<DomainSeries>>>> Handle(GetAllSeriesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<DomainSeries>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }
}