using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.CoverImageMetaData.GetCoverImageMetaDataById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.CoverImageMetaData;

public sealed class CoverImageMetaDataQueryHandler :
    IQueryHandler<GetCoverImageMetaDataByIdQuery, CoverImageMetaDataContainer>
{
    private readonly ICoverImageMetaDataReadOnlyRepository _repository;

    public CoverImageMetaDataQueryHandler(ICoverImageMetaDataReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CoverImageMetaDataContainer>> Handle(GetCoverImageMetaDataByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsContainerAsync(request.CoverImageId, cancellationToken);
    }
}