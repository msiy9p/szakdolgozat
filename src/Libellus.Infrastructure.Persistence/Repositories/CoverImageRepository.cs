﻿using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Libellus.Domain.Errors;
using Libellus.Domain.ValueObjects;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class CoverImageRepository : BaseMinioRepository<CoverImageRepository>, ICoverImageRepository
{
    public CoverImageRepository(MinioClient minioClient, ILogger<CoverImageRepository> logger) : base(minioClient,
        logger)
    {
    }

    public async Task<Result<bool>> ExistsAsync(string objectName, CancellationToken cancellationToken = default)
    {
        var probe = ObjectName.Probe(objectName);

        if (probe.IsError)
        {
            return Result<bool>.Error(probe.Errors);
        }

        var statObjectArgs = new StatObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName);

        try
        {
            await MinioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (MinioException e)
        {
            return Result<bool>.Success(false);
        }
    }

    public async Task<Result<bool>> ExistsAsync(CoverImageId id, CancellationToken cancellationToken = default)
    {
        var args = new ListObjectsArgs()
            .WithBucket(BucketName)
            .WithPrefix(id + "/");

        var content = await MinioClient.ListObjectsAsync(args, cancellationToken).ToList().ToTask(cancellationToken);

        return Result<bool>.Success(content.Any());
    }

    public async Task<Result<CoverImage>> GetByObjectNameAsync(string objectName,
        CancellationToken cancellationToken = default)
    {
        var probe = ObjectName.Probe(objectName);

        if (probe.IsError)
        {
            return Result<CoverImage>.Error(probe.Errors);
        }

        using (var memoryStream = new MemoryStream())
        {
            var args = new GetObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithCallbackStream(x => x.CopyTo(memoryStream));

            try
            {
                await MinioClient.GetObjectAsync(args, cancellationToken);

                byte[] data = memoryStream.ToArray();
                return CoverImage.Create(objectName, data);
            }
            catch (MinioException e)
            {
                Logger.LogError(e, "Could not get CoverImage with name {ObjectName}.", objectName);
                return Result<CoverImage>.Error(DomainErrors.CoverImageErrors.CoverImageNotFound);
            }
        }
    }

    public async Task<Result> AddIfNotExistsAsync(CoverImage image, CancellationToken cancellationToken = default)
    {
        if (image is null)
        {
            return Result.Error(DomainErrors.GeneralErrors.InputIsNull);
        }

        var exists = await ExistsAsync(image.ToString(), cancellationToken);

        if (exists.IsError)
        {
            return exists;
        }

        if (exists.Value)
        {
            return Result.Success();
        }

        using (var memoryStream = new MemoryStream(image.Data))
        {
            var args = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(image.ToString())
                .WithObjectSize(image.Data.LongLength)
                .WithContentType(ImageFormatExtensions.ToMimeType(image.ImageFormat).Value)
                .WithStreamData(memoryStream);

            try
            {
                await MinioClient.PutObjectAsync(args, cancellationToken);

                return Result.Success();
            }
            catch (MinioException e)
            {
                Logger.LogError(e, "Could not create CoverImage with name {ObjectName}.", image.ToString());
                return Result.Error();
            }
        }
    }

    public async Task<Result> DeleteByObjectNameAsync(string objectName, CancellationToken cancellationToken = default)
    {
        var probe = ObjectName.Probe(objectName);

        if (probe.IsError)
        {
            return Result.Error(probe.Errors);
        }

        var args = new RemoveObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName);

        try
        {
            await MinioClient.RemoveObjectAsync(args, cancellationToken);

            return Result.Success();
        }
        catch (MinioException e)
        {
            Logger.LogError(e, "Could not delete CoverImage with name {ObjectName}.", objectName);
            return Result.Error();
        }
    }

    public async Task<Result> DeleteByIdAsync(CoverImageId id, CancellationToken cancellationToken = default)
    {
        var args = new ListObjectsArgs()
            .WithBucket(BucketName)
            .WithPrefix(id + "/");

        var content = await MinioClient.ListObjectsAsync(args, cancellationToken).ToList().ToTask(cancellationToken);

        if (content.Count > 0)
        {
            var args2 = new RemoveObjectsArgs()
                .WithBucket(BucketName)
                .WithObjects(
                    content.Where(x => !x.IsDir)
                        .Select(x => x.Key)
                        .ToList());

            var observable = await MinioClient.RemoveObjectsAsync(args2, cancellationToken);
            var deleteErrors = await observable.ToList().ToTask(cancellationToken);

            if (deleteErrors.Any())
            {
                Logger.LogError("Could not delete CoverImage with id {CoverImageId}.", id);
                foreach (var deleteError in deleteErrors)
                {
                    Logger.LogError("{CoverImageId}, message: {Message}", id, deleteError.Message);
                }

                return Result.Error();
            }
        }

        return Result.Success();
    }
}