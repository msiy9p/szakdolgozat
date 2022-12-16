﻿using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.Entities;

public sealed class CoverImage : BaseEntity<CoverImageId>, IImageDataOnly
{
    private ObjectName? _objectName;

    public int Width { get; init; }
    public ImageFormat ImageFormat { get; init; }
    public byte[] Data { get; init; }

    internal CoverImage(CoverImageId id, int width, ImageFormat imageFormat, byte[] data) : base(id)
    {
        Width = Guard.Against.NegativeOrZero(width);
        ImageFormat = Guard.Against.ImageFormatOutOfRange(imageFormat);
        Data = Guard.Against.Null(data);
        Guard.Against.Zero(Data.Length);
    }

    public static Result<CoverImage> Create(CoverImageId id, int width, ImageFormat imageFormat, byte[] data)
    {
        var result = Create(id);
        if (result.IsError)
        {
            return Result<CoverImage>.Invalid(result.Errors);
        }

        if (width <= 0)
        {
            return Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageSizeWidthNotSupported);
        }

        if (!ImageFormatExtensions.IsDefined(imageFormat))
        {
            return Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageFormatNotSupported);
        }

        if (data is null || data.Length <= 0)
        {
            Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageDataNotValid);
        }

        return Result<CoverImage>.Success(new CoverImage(id, width, imageFormat, data!));
    }

    public static Result<CoverImage> Create(string objectName, byte[] data)
    {
        if (data is null || data.Length <= 0)
        {
            Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageDataNotValid);
        }

        if (string.IsNullOrWhiteSpace(objectName))
        {
            Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageObjectNameNotValid);
        }

        if (!Guid.TryParse(objectName!.Substring(0, 36), out var guidId))
        {
            Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageObjectNameNotValid);
        }

        string[] temp = objectName.Substring(37).Split("w.", 2, StringSplitOptions.TrimEntries);

        if (!int.TryParse(temp[0], out var width))
        {
            Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageObjectNameNotValid);
        }

        if (string.IsNullOrWhiteSpace(temp[1]))
        {
            Result<CoverImage>.Invalid(DomainErrors.ImageErrors.ImageObjectNameNotValid);
        }

        var imageFormat = ImageFormatExtensions.FromExtension(temp[1]);

        if (imageFormat.IsError)
        {
            Result<CoverImage>.Invalid(imageFormat.Errors);
        }

        return Create(new CoverImageId(guidId), width, imageFormat.Value, data!);
    }

    public ObjectName GetObjectName() => _objectName ??= new ObjectName(ToString());

    public override string ToString()
    {
        var result = ImageFormatExtensions.ToExtension(ImageFormat);

        if (result.IsSuccess)
        {
            return $"{Id:N}-{Width}w{result.Value}";
        }

        return $"{Id:N}-{Width}w";
    }
}