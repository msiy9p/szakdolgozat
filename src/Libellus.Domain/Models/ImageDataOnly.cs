using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Errors;
using Libellus.Domain.Utilities;

namespace Libellus.Domain.Models;

public sealed class ImageDataOnly : IImageDataOnly
{
    public byte[] Data { get; init; }

    public ImageDataOnly(byte[] data)
    {
        Data = Guard.Against.Null(data);
        Guard.Against.Zero(Data.Length);
    }

    public static Result<ImageDataOnly> Create(byte[] data)
    {
        if (data is null || data.Length <= 0)
        {
            return DomainErrors.ImageErrors.ImageDataNotValid.ToInvalidResult<ImageDataOnly>();
        }

        return new ImageDataOnly(data).ToResult();
    }
}