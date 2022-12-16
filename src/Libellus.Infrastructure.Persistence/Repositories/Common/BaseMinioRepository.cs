using Microsoft.Extensions.Logging;
using Minio;

namespace Libellus.Infrastructure.Persistence.Repositories.Common;

internal abstract class BaseMinioRepository
{
    protected internal const string BucketName = "libellus-images";

    protected readonly MinioClient MinioClient;
    protected readonly ILogger Logger;

    protected BaseMinioRepository(MinioClient minioClient, ILogger logger)
    {
        MinioClient = minioClient;
        Logger = logger;
    }
}

internal abstract class BaseMinioRepository<T> : BaseMinioRepository where T : BaseMinioRepository
{
    protected BaseMinioRepository(MinioClient minioClient, ILogger<T> logger) : base(minioClient, logger)
    {
    }
}