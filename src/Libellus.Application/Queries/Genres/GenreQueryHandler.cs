using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Genres.GetAllGenres;
using Libellus.Application.Queries.Genres.GetGenreById;
using Libellus.Application.Queries.Genres.GetGenreByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Genres;

public sealed class GenreQueryHandler :
    IQueryHandler<GetAllGenresQuery, ICollection<Genre>>,
    IQueryHandler<GetGenreByIdQuery, Genre>,
    IQueryHandler<GetGenreByNameQuery, Genre>
{
    private readonly IGenreReadOnlyRepository _repository;

    public GenreQueryHandler(IGenreReadOnlyRepository genreRepository)
    {
        _repository = genreRepository;
    }

    public async Task<Result<ICollection<Genre>>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<Genre>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.GenreId, cancellationToken: cancellationToken);
    }

    public async Task<Result<Genre>> Handle(GetGenreByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}