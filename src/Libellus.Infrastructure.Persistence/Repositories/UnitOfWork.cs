using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Domain.Models;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ApplicationContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while saving unit-of-work.");
            throw;
        }
    }
}