using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using DomainProfilePictureMetaData = Libellus.Domain.Entities.ProfilePictureMetaData;
using PersistenceProfilePictureMetaData = Libellus.Infrastructure.Persistence.DataModels.ProfilePictureMetaData;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct ProfilePictureMetaDataMapper : IMapFrom<PersistenceProfilePictureMetaData, Result<DomainProfilePictureMetaData>>, IMapFrom<ProfilePictureMetaData, PersistenceProfilePictureMetaData>
{
    public static Result<DomainProfilePictureMetaData> Map(PersistenceProfilePictureMetaData item)
    {
        var imageFormat = ImageFormatExtensions.FromMimeType(item.MimeType);

        if (imageFormat.IsError)
        {
            return Result<DomainProfilePictureMetaData>.Error(imageFormat.Errors);
        }

        return DomainProfilePictureMetaData.Create(ProfilePictureId.Convert(item.PublicId)!.Value, item.Width,
            item.Height, imageFormat.Value, item.DataSize, item.CreatedOnUtc);
    }

    public static PersistenceProfilePictureMetaData Map(DomainProfilePictureMetaData item)
    {
        return new PersistenceProfilePictureMetaData
        {
            PublicId = item.Id.Value,
            Width = item.Width,
            Height = item.Height,
            DataSize = item.DataSize,
            MimeType = ImageFormatExtensions.ToMimeType(item.ImageFormat).Value!,
            ObjectName = item.ToString(),
            CreatedOnUtc = item.CreatedOnUtc,
        };
    }
}