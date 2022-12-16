using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.Repositories.Cached;

internal sealed class CachedFriendlyIdLookupRepository : IFriendlyIdLookupRepository
{
    private readonly FriendlyIdLookupRepository _original;
    private readonly ILogger<CachedFriendlyIdLookupRepository> _logger;
    private readonly IDistributedCache _distributedCache;

    private static readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public CachedFriendlyIdLookupRepository(FriendlyIdLookupRepository original,
        ILogger<CachedFriendlyIdLookupRepository> logger, IDistributedCache distributedCache)
    {
        _original = original;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    private static string CreateCacheId<T>(T friendlyId, string friendlyIdValue) where T : IFriendlyIdType
    {
        return $"{nameof(T)}:{friendlyIdValue}";
    }

    public Result<AuthorId> Lookup(AuthorFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new AuthorId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<BookEditionId> Lookup(BookEditionFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new BookEditionId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<BookId> Lookup(BookFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new BookId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<CommentId> Lookup(CommentFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new CommentId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<GroupId> Lookup(GroupFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new GroupId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<PostId> Lookup(PostFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new PostId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<ReadingId> Lookup(ReadingFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new ReadingId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<SeriesId> Lookup(SeriesFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new SeriesId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public Result<ShelfId> Lookup(ShelfFriendlyId friendlyId)
    {
        var cachedValue = _distributedCache.Get(CreateCacheId(friendlyId, friendlyId.Value));
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = _original.Lookup(friendlyId);
            if (result.IsError)
            {
                return result;
            }

            _distributedCache.Set(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions);

            return result;
        }

        var id = new ShelfId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<AuthorId>> LookupAsync(AuthorFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new AuthorId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<BookEditionId>> LookupAsync(BookEditionFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new BookEditionId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<BookId>> LookupAsync(BookFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new BookId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<CommentId>> LookupAsync(CommentFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new CommentId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<GroupId>> LookupAsync(GroupFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new GroupId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<PostId>> LookupAsync(PostFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new PostId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<ReadingId>> LookupAsync(ReadingFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new ReadingId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<SeriesId>> LookupAsync(SeriesFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new SeriesId(new Guid(cachedValue!));
        return id.ToResult();
    }

    public async Task<Result<ShelfId>> LookupAsync(ShelfFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var cachedValue =
            await _distributedCache.GetAsync(CreateCacheId(friendlyId, friendlyId.Value), cancellationToken);
        if (cachedValue is null || cachedValue.Length != 16)
        {
            var result = await _original.LookupAsync(friendlyId, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _distributedCache.SetAsync(CreateCacheId(friendlyId, friendlyId.Value),
                result.Value.Value.ToByteArray(),
                _cacheOptions, cancellationToken);

            return result;
        }

        var id = new ShelfId(new Guid(cachedValue!));
        return id.ToResult();
    }
}