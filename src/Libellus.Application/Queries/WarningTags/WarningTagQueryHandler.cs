using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.WarningTags.GetAllWarningTags;
using Libellus.Application.Queries.WarningTags.GetWarningTagById;
using Libellus.Application.Queries.WarningTags.GetWarningTagByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.WarningTags;

public sealed class WarningTagQueryHandler :
    IQueryHandler<GetAllWarningTagsQuery, ICollection<WarningTag>>,
    IQueryHandler<GetWarningTagByIdQuery, WarningTag>,
    IQueryHandler<GetWarningTagByNameQuery, WarningTag>
{
    private readonly IWarningTagReadOnlyRepository _repository;

    public WarningTagQueryHandler(IWarningTagReadOnlyRepository WarningTagRepository)
    {
        _repository = WarningTagRepository;
    }

    public async Task<Result<ICollection<WarningTag>>> Handle(GetAllWarningTagsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<WarningTag>> Handle(GetWarningTagByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.WarningTagId, cancellationToken: cancellationToken);
    }

    public async Task<Result<WarningTag>> Handle(GetWarningTagByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}