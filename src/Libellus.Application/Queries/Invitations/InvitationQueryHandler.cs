using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Invitations.GetInvitationById;
using Libellus.Application.Queries.Invitations.GetInvitationsByGroup;
using Libellus.Application.Queries.Invitations.GetInvitationsByInvitee;
using Libellus.Application.Queries.Invitations.GetInvitationVmsByGroup;
using Libellus.Application.Queries.Invitations.GetInvitationVmsByInvitee;
using Libellus.Application.ViewModels;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Queries.Invitations;

public sealed class InvitationQueryHandler :
    IQueryHandler<GetInvitationByIdQuery, Invitation>,
    IQueryHandler<GetInvitationsByGroupQuery, ICollection<Invitation>>,
    IQueryHandler<GetInvitationsByInviteeQuery, ICollection<Invitation>>,
    IQueryHandler<GetInvitationVmsByGroupQuery, ICollection<InvitationPicturedVm>>,
    IQueryHandler<GetInvitationVmsByInviteeQuery, ICollection<InvitationUserVm>>
{
    private readonly IInvitationReadOnlyRepository _repository;

    public InvitationQueryHandler(IInvitationReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Invitation>> Handle(GetInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.InvitationId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Invitation>>> Handle(GetInvitationsByGroupQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllByGroupIdAsync(request.GroupId, request.InvitationStatus, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Invitation>>> Handle(GetInvitationsByInviteeQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllByInviteeIdAsync(request.UserId, request.InvitationStatus, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<InvitationPicturedVm>>> Handle(GetInvitationVmsByGroupQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllPicturedVmByGroupIdAsync(request.GroupId, request.InvitationStatus,
            request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<InvitationUserVm>>> Handle(GetInvitationVmsByInviteeQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllVmByInviteeIdAsync(request.UserId, request.InvitationStatus,
            request.SortOrder, cancellationToken: cancellationToken);
    }
}