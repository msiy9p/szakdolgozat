using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using DomainReading = Libellus.Domain.Entities.Reading;

namespace Libellus.Infrastructure.Persistence.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class PostLockJob : IJob
{
    public static readonly JobKey JobKey = new(nameof(PostLockJob));

    private readonly ApplicationContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<PostLockJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PostLockJob(ApplicationContext context, IDateTimeProvider dateTimeProvider,
        ILogger<PostLockJob> logger, IUnitOfWork unitOfWork)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var lockReason = $"Post automatically locked after {(int)DomainReading.ExpireAfterDays.TotalDays} days.";

        var stop = false;
        do
        {
            var found = await _context.Posts
                .Where(x => (dateTime - x.CreatedOnUtc).TotalDays > DomainReading.ExpireAfterDays.TotalDays)
                .Select(x => x.Id)
                .Take(20)
                .ToListAsync(context.CancellationToken);

            if (found.Count == 0)
            {
                _logger.LogInformation("No posts found to automatically lock.");
                stop = true;
            }
            else
            {
                _logger.LogInformation("{PostCount} posts found to automatically lock.", found.Count);
                foreach (var postId in found)
                {
                    _logger.LogInformation("Automatically locking post {PostId}.", postId);
                    await _context.LockedPosts
                        .AddAsync(new LockedPost(postId, null, lockReason, dateTime), context.CancellationToken);
                }
            }
        } while (!stop);

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}