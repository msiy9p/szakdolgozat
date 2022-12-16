using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.LiteratureForms.GetAllLiteratureForms;
using Libellus.Application.Queries.LiteratureForms.GetLiteratureFormById;
using Libellus.Application.Queries.LiteratureForms.GetLiteratureFormByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.LiteratureForms;

public sealed class LiteratureFormQueryHandler :
    IQueryHandler<GetAllLiteratureFormsQuery, ICollection<LiteratureForm>>,
    IQueryHandler<GetLiteratureFormByIdQuery, LiteratureForm>,
    IQueryHandler<GetLiteratureFormByNameQuery, LiteratureForm>
{
    private readonly ILiteratureFormReadOnlyRepository _repository;

    public LiteratureFormQueryHandler(ILiteratureFormReadOnlyRepository LiteratureFormRepository)
    {
        _repository = LiteratureFormRepository;
    }

    public async Task<Result<ICollection<LiteratureForm>>> Handle(GetAllLiteratureFormsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<LiteratureForm>> Handle(GetLiteratureFormByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.LiteratureFormId, cancellationToken: cancellationToken);
    }

    public async Task<Result<LiteratureForm>> Handle(GetLiteratureFormByNameQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}