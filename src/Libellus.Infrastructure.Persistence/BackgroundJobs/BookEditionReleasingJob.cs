using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Libellus.Infrastructure.Persistence.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class BookEditionReleasingJob : IJob
{
    public static readonly JobKey JobKey = new(nameof(BookEditionReleasingJob));

    private readonly IBookEditionReleasingRepository _repository;
    private readonly IPublisher _publisher;
    private readonly ILogger<BookEditionReleasingJob> _logger;
    private readonly IDateTimeProvider _dueTimeProvider;

    public BookEditionReleasingJob(IBookEditionReleasingRepository repository, IPublisher publisher,
        ILogger<BookEditionReleasingJob> logger, IDateTimeProvider dueTimeProvider)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
        _dueTimeProvider = dueTimeProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var found = await _repository.GetAllIdsReleasingAsync(context.CancellationToken);
        if (found.IsError)
        {
            return;
        }

        foreach (var bookEditionId in found.Value!)
        {
            var releaseEvent = new BookEditionReleasingEvent(_dueTimeProvider.UtcNow, bookEditionId);
            await _publisher.Publish(releaseEvent, context.CancellationToken);
        }
    }
}