using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.Repositories.Common;

internal abstract class BaseRepository
{
    protected readonly ApplicationContext Context;
    protected readonly ILogger Logger;

    protected BaseRepository(ApplicationContext context, ILogger logger)
    {
        Context = context;
        Logger = logger;
    }
}

internal abstract class BaseRepository<T> : BaseRepository where T : BaseRepository
{
    protected BaseRepository(ApplicationContext context, ILogger<T> logger) : base(context, logger)
    {
    }

    protected BaseRepository(ApplicationContext context, ILogger logger) : base(context, logger)
    {
    }
}