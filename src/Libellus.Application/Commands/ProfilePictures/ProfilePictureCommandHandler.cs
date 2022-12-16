using Libellus.Application.Commands.ProfilePictures.CreateProfilePictures;
using Libellus.Application.Commands.ProfilePictures.DeleteProfilePicturesById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.ProfilePictures;

public sealed class ProfilePictureCommandHandler :
    ICommandHandler<DeleteProfilePicturesByIdCommand>,
    ICommandHandler<CreateProfilePicturesCommand, ProfilePictureMetaDataContainer>
{
    private readonly IImageResizerWithPreference _imageResizer;
    private readonly IProfilePictureMetaDataRepository _metaDataRepository;
    private readonly IProfilePictureRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProfilePictureCommandHandler(IImageResizerWithPreference imageResizer,
        IProfilePictureMetaDataRepository profilePictureMetaDataRepository,
        IProfilePictureRepository profilePictureRepository, IUnitOfWork unitOfWork)
    {
        _imageResizer = imageResizer;
        _metaDataRepository = profilePictureMetaDataRepository;
        _repository = profilePictureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProfilePicturesByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.ExistsAsync(request.ProfilePictureId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (result.Value)
        {
            var ciDeleteResult = await _repository.DeleteByIdAsync(request.ProfilePictureId, cancellationToken);
            if (ciDeleteResult.IsError)
            {
                return ciDeleteResult;
            }
        }

        var metaResult = await _metaDataRepository.ExistsAsync(request.ProfilePictureId, cancellationToken);
        if (metaResult.IsError)
        {
            return metaResult;
        }

        if (metaResult.Value)
        {
            var metaDeleteResult = await _metaDataRepository.DeleteAsync(request.ProfilePictureId, cancellationToken);
            if (metaDeleteResult.IsError)
            {
                return metaDeleteResult;
            }
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<ProfilePictureMetaDataContainer>> Handle(CreateProfilePicturesCommand request,
        CancellationToken cancellationToken)
    {
        var imagesResult = await _imageResizer
            .ResizeToAllWidthsAsync(request.ImageDataOnly, _imageResizer.PreferredImageFormat, cancellationToken);

        if (imagesResult.IsError)
        {
            return Result<ProfilePictureMetaDataContainer>.Error(imagesResult.Errors);
        }

        var id = ProfilePictureId.Create();
        var metaData = new List<ProfilePictureMetaData>(imagesResult.Value!.Count);

        foreach (var imageComplete in imagesResult.Value!)
        {
            var image = ProfilePicture.Create(id, imageComplete.Width, imageComplete.ImageFormat, imageComplete.Data);
            if (image.IsError)
            {
                return Result<ProfilePictureMetaDataContainer>.Error(image.Errors);
            }

            var addResult = await _repository.AddIfNotExistsAsync(image.Value!, cancellationToken);
            if (addResult.IsError)
            {
                return Result<ProfilePictureMetaDataContainer>.Error(addResult.Errors);
            }

            var meta = ProfilePictureMetaData.Create(id, imageComplete.Width, imageComplete.Height,
                imageComplete.ImageFormat, imageComplete.DataSize, imageComplete.CreatedOnUtc);
            if (meta.IsError)
            {
                return Result<ProfilePictureMetaDataContainer>.Error(meta.Errors);
            }

            metaData.Add(meta.Value!);
        }

        var metaContainer = new ProfilePictureMetaDataContainer(id, metaData);

        var metaAddResult = await _metaDataRepository.AddAsync(metaContainer, cancellationToken);
        if (metaAddResult.IsError)
        {
            return Result<ProfilePictureMetaDataContainer>.Error(metaAddResult.Errors);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (result.IsError)
        {
            return Result<ProfilePictureMetaDataContainer>.Error(result.Errors);
        }

        return Result<ProfilePictureMetaDataContainer>.Success(metaContainer);
    }
}