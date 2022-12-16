using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Tags.GetAllTags;
using Libellus.Application.Queries.Tags.GetTagById;
using Libellus.Application.Queries.Tags.GetTagByName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Tags;

public sealed class TagQueryHandler :
    IQueryHandler<GetAllTagsQuery, ICollection<Tag>>,
    IQueryHandler<GetTagByIdQuery, Tag>,
    IQueryHandler<GetTagByNameQuery, Tag>
{
    private readonly ITagReadOnlyRepository _repository;

    public TagQueryHandler(ITagReadOnlyRepository TagRepository)
    {
        _repository = TagRepository;
    }

    public async Task<Result<ICollection<Tag>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<Tag>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.TagId, cancellationToken: cancellationToken);
    }

    public async Task<Result<Tag>> Handle(GetTagByNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.FindByNameAsync(request.Name, cancellationToken: cancellationToken);
    }
}