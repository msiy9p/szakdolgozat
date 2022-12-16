using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Publishers.GetAllPublishers;
using Libellus.Application.Queries.Publishers.GetPublisherById;
using Libellus.Application.Queries.Publishers.GetPublisherByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Publishers;

public sealed class PublisherQueryHandler :
    IQueryHandler<GetAllPublishersQuery, ICollection<Publisher>>,
    IQueryHandler<GetPublisherByIdQuery, Publisher>,
    IQueryHandler<GetPublisherByNameQuery, Publisher>
{
    private readonly IPublisherReadOnlyRepository _repository;

    public PublisherQueryHandler(IPublisherReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ICollection<Publisher>>> Handle(GetAllPublishersQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<Publisher>> Handle(GetPublisherByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.PublisherId, cancellationToken: cancellationToken);
    }

    public async Task<Result<Publisher>> Handle(GetPublisherByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}