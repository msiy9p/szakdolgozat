using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.InvitationRequests.GetInvitationRequestById;
using Libellus.Application.Queries.InvitationRequests.GetInvitationRequestsByGroup;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.InvitationRequests;

public sealed class InvitationQueryHandler :
    IQueryHandler<GetInvitationRequestByIdQuery, InvitationRequest>,
    IQueryHandler<GetInvitationRequestsByGroupQuery, ICollection<InvitationRequest>>
{
    private readonly IInvitationRequestReadOnlyRepository _repository;

    public InvitationQueryHandler(IInvitationRequestReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<InvitationRequest>> Handle(GetInvitationRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.InvitationId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<InvitationRequest>>> Handle(GetInvitationRequestsByGroupQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllByGroupIdAsync(request.GroupId, request.InvitationStatus, request.SortOrder,
            cancellationToken: cancellationToken);
    }
}