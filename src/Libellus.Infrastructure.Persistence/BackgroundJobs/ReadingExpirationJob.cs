using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using DomainReading = Libellus.Domain.Entities.Reading;

namespace Libellus.Infrastructure.Persistence.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ReadingExpirationJob : IJob
{
    public static readonly JobKey JobKey = new(nameof(ReadingExpirationJob));

    private readonly ApplicationContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ReadingExpirationJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ReadingExpirationJob(ApplicationContext context, IDateTimeProvider dateTimeProvider,
        ILogger<ReadingExpirationJob> logger, IUnitOfWork unitOfWork)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dateTime = _dateTimeProvider.UtcNow;

        await _context.Readings
            .Where(x => x.StartedOnUtc == null)
            .Where(x => (dateTime - x.CreatedOnUtc).TotalDays > DomainReading.ExpireAfterDays.TotalDays)
            .ExecuteDeleteAsync(context.CancellationToken);

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}