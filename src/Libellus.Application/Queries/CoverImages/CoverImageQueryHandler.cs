using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.CoverImages.GetCoverImageByObjectName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.CoverImages;

public sealed class CoverImageQueryHandler :
    IQueryHandler<GetCoverImageByObjectNameQuery, CoverImage>
{
    private readonly ICoverImageReadOnlyRepository _repository;

    public CoverImageQueryHandler(ICoverImageReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CoverImage>> Handle(GetCoverImageByObjectNameQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByObjectNameAsync(request.ObjectName, cancellationToken);
    }
}