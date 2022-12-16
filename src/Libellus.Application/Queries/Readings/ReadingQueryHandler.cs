using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Readings.GetAllReadingsPaginated;
using Libellus.Application.Queries.Readings.GetReadingById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Readings;

public sealed class ReadingQueryHandler :
    IQueryHandler<GetReadingByIdQuery, Reading>,
    IQueryHandler<GetAllReadingsPaginatedQuery, PaginationDetail<ICollection<Reading>>>
{
    private readonly IReadingReadOnlyRepository _repository;

    public ReadingQueryHandler(IReadingReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Reading>> Handle(GetReadingByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.ReadingId, cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Reading>>>> Handle(GetAllReadingsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Reading>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder, cancellationToken);
    }
}