using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Labels.GetAllLabels;
using Libellus.Application.Queries.Labels.GetLabelById;
using Libellus.Application.Queries.Labels.GetLabelByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Labels;

public sealed class LabelQueryHandler :
    IQueryHandler<GetAllLabelsQuery, ICollection<Label>>,
    IQueryHandler<GetLabelByIdQuery, Label>,
    IQueryHandler<GetLabelByNameQuery, Label>
{
    private readonly ILabelReadOnlyRepository _repository;

    public LabelQueryHandler(ILabelReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ICollection<Label>>> Handle(GetAllLabelsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<Label>> Handle(GetLabelByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.LabelId, cancellationToken: cancellationToken);
    }

    public async Task<Result<Label>> Handle(GetLabelByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}