using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Formats.GetAllFormats;
using Libellus.Application.Queries.Formats.GetFormatById;
using Libellus.Application.Queries.Formats.GetFormatByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Formats;

public sealed class FormatQueryHandler :
    IQueryHandler<GetAllFormatsQuery, ICollection<Format>>,
    IQueryHandler<GetFormatByIdQuery, Format>,
    IQueryHandler<GetFormatByNameQuery, Format>
{
    private readonly IFormatReadOnlyRepository _repository;

    public FormatQueryHandler(IFormatReadOnlyRepository genreRepository)
    {
        _repository = genreRepository;
    }

    public async Task<Result<ICollection<Format>>> Handle(GetAllFormatsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<Format>> Handle(GetFormatByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.FormatId, cancellationToken: cancellationToken);
    }

    public async Task<Result<Format>> Handle(GetFormatByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}