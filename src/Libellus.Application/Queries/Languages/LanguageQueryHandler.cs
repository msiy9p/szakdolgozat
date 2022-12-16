using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Languages.GetAllLanguages;
using Libellus.Application.Queries.Languages.GetLanguageById;
using Libellus.Application.Queries.Languages.GetLanguageByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Languages;

public sealed class LanguageQueryHandler :
    IQueryHandler<GetAllLanguagesQuery, ICollection<Language>>,
    IQueryHandler<GetLanguageByIdQuery, Language>,
    IQueryHandler<GetLanguageByNameQuery, Language>
{
    private readonly ILanguageReadOnlyRepository _repository;

    public LanguageQueryHandler(ILanguageReadOnlyRepository LanguageRepository)
    {
        _repository = LanguageRepository;
    }

    public async Task<Result<ICollection<Language>>> Handle(GetAllLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken);
    }

    public async Task<Result<Language>> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.LanguageId, cancellationToken);
    }

    public async Task<Result<Language>> Handle(GetLanguageByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken);
    }
}