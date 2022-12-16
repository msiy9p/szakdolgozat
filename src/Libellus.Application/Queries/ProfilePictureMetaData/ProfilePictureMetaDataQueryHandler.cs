using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.ProfilePictureMetaData.GetProfilePictureMetaDataById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.ProfilePictureMetaData;

public sealed class ProfilePictureMetaDataQueryHandler :
    IQueryHandler<GetProfilePictureMetaDataByIdQuery, ProfilePictureMetaDataContainer>
{
    private readonly IProfilePictureMetaDataReadOnlyRepository _repository;

    public ProfilePictureMetaDataQueryHandler(IProfilePictureMetaDataReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProfilePictureMetaDataContainer>> Handle(GetProfilePictureMetaDataByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsContainerAsync(request.ProfilePictureId, cancellationToken);
    }
}