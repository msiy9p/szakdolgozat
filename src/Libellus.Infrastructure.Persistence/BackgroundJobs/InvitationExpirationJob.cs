using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Libellus.Infrastructure.Persistence.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class InvitationExpirationJob : IJob
{
    public static readonly JobKey JobKey = new(nameof(InvitationExpirationJob));

    private readonly IInvitationRepository _invitationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<InvitationExpirationJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public InvitationExpirationJob(IInvitationRepository invitationRepository, IDateTimeProvider dateTimeProvider,
        ILogger<InvitationExpirationJob> logger, IUnitOfWork unitOfWork)
    {
        _invitationRepository = invitationRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var updated = false;
        var results =
            await _invitationRepository.GetAllToExpireAsync(_dateTimeProvider, SortOrder.Ascending,
                context.CancellationToken);

        if (results.IsError)
        {
            return;
        }

        foreach (var result in results.Value!)
        {
            result.Expire(_dateTimeProvider);
            await _invitationRepository.UpdateAsync(result, context.CancellationToken);
            updated = true;
        }

        if (updated)
        {
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }
}