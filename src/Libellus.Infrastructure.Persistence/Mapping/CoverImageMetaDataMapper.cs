using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using PersistenceCoverImageMetaData = Libellus.Infrastructure.Persistence.DataModels.CoverImageMetaData;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct CoverImageMetaDataMapper : IMapFrom<PersistenceCoverImageMetaData, Result<CoverImageMetaData>>, IMapFrom<CoverImageMetaData, PersistenceCoverImageMetaData>
{
    public static Result<CoverImageMetaData> Map(PersistenceCoverImageMetaData item1)
    {
        var imageFormat = ImageFormatExtensions.FromMimeType(item1.MimeType);

        if (imageFormat.IsError)
        {
            return Result<CoverImageMetaData>.Error(imageFormat.Errors);
        }

        return CoverImageMetaData.Create(CoverImageId.Convert(item1.PublicId)!.Value, item1.Width, item1.Height,
            imageFormat.Value, item1.DataSize, item1.CreatedOnUtc);
    }

    public static PersistenceCoverImageMetaData Map(CoverImageMetaData item1)
    {
        return new PersistenceCoverImageMetaData
        {
            PublicId = item1.Id.Value,
            Width = item1.Width,
            Height = item1.Height,
            DataSize = item1.DataSize,
            MimeType = ImageFormatExtensions.ToMimeType(item1.ImageFormat).Value!,
            ObjectName = item1.ToString(),
            CreatedOnUtc = item1.CreatedOnUtc,
        };
    }
}