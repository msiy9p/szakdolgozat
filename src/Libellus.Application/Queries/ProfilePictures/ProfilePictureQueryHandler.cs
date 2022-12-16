using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.ProfilePictures.GetProfilePictureByObjectName;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.ProfilePictures;

public sealed class ProfilePictureQueryHandler :
    IQueryHandler<GetProfilePictureByObjectNameQuery, ProfilePicture>
{
    private readonly IProfilePictureReadOnlyRepository _repository;

    public ProfilePictureQueryHandler(IProfilePictureReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProfilePicture>> Handle(GetProfilePictureByObjectNameQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByObjectNameAsync(request.ObjectName, cancellationToken);
    }
}