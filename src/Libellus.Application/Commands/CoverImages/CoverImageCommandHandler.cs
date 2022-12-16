using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Commands.CoverImages.DeleteCoverImagesById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.CoverImages;

public sealed class CoverImageCommandHandler :
    ICommandHandler<DeleteCoverImagesByIdCommand>,
    ICommandHandler<CreateCoverImagesCommand, CoverImageMetaDataContainer>
{
    private readonly IImageResizerWithPreference _imageResizer;
    private readonly ICoverImageMetaDataRepository _metaDataRepository;
    private readonly ICoverImageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CoverImageCommandHandler(IImageResizerWithPreference imageResizer,
        ICoverImageMetaDataRepository coverImageMetaDataRepository, ICoverImageRepository coverImageRepository,
        IUnitOfWork unitOfWork)
    {
        _imageResizer = imageResizer;
        _metaDataRepository = coverImageMetaDataRepository;
        _repository = coverImageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCoverImagesByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.ExistsAsync(request.CoverImageId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (result.Value)
        {
            var ciDeleteResult = await _repository.DeleteByIdAsync(request.CoverImageId, cancellationToken);
            if (ciDeleteResult.IsError)
            {
                return ciDeleteResult;
            }
        }

        var metaResult = await _metaDataRepository.ExistsAsync(request.CoverImageId, cancellationToken);
        if (metaResult.IsError)
        {
            return metaResult;
        }

        if (metaResult.Value)
        {
            var metaDeleteResult = await _metaDataRepository.DeleteAsync(request.CoverImageId, cancellationToken);
            if (metaDeleteResult.IsError)
            {
                return metaDeleteResult;
            }
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<CoverImageMetaDataContainer>> Handle(CreateCoverImagesCommand request,
        CancellationToken cancellationToken)
    {
        var imagesResult = await _imageResizer
            .ResizeToAllWidthsAsync(request.ImageDataOnly, _imageResizer.PreferredImageFormat, cancellationToken);

        if (imagesResult.IsError)
        {
            return Result<CoverImageMetaDataContainer>.Error(imagesResult.Errors);
        }

        var id = CoverImageId.Create();
        var metaData = new List<CoverImageMetaData>(imagesResult.Value!.Count);

        foreach (var imageComplete in imagesResult.Value!)
        {
            var image = CoverImage.Create(id, imageComplete.Width, imageComplete.ImageFormat, imageComplete.Data);
            if (image.IsError)
            {
                return Result<CoverImageMetaDataContainer>.Error(image.Errors);
            }

            var addResult = await _repository.AddIfNotExistsAsync(image.Value!, cancellationToken);
            if (addResult.IsError)
            {
                return Result<CoverImageMetaDataContainer>.Error(addResult.Errors);
            }

            var meta = CoverImageMetaData.Create(id, imageComplete.Width, imageComplete.Height,
                imageComplete.ImageFormat, imageComplete.DataSize, imageComplete.CreatedOnUtc);
            if (meta.IsError)
            {
                return Result<CoverImageMetaDataContainer>.Error(meta.Errors);
            }

            metaData.Add(meta.Value!);
        }

        var metaContainer = new CoverImageMetaDataContainer(id, metaData);

        var metaAddResult = await _metaDataRepository.AddAsync(metaContainer, cancellationToken);
        if (metaAddResult.IsError)
        {
            return Result<CoverImageMetaDataContainer>.Error(metaAddResult.Errors);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (result.IsError)
        {
            return Result<CoverImageMetaDataContainer>.Error(result.Errors);
        }

        return Result<CoverImageMetaDataContainer>.Success(metaContainer);
    }
}