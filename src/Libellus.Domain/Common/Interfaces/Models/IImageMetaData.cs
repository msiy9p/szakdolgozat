using Libellus.Domain.Enums;
using NodaTime;

namespace Libellus.Domain.Common.Interfaces.Models;

public interface IImageMetaData
{
    int Width { get; }
    int Height { get; }
    ImageFormat ImageFormat { get; }
    int DataSize { get; }
    ZonedDateTime CreatedOnUtc { get; }
}