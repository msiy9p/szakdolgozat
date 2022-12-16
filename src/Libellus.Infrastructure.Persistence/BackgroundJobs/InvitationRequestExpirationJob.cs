using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Libellus.Infrastructure.Persistence.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class InvitationRequestExpirationJob : IJob
{
    public static readonly JobKey JobKey = new(nameof(InvitationRequestExpirationJob));

    private readonly IInvitationRequestRepository _invitationRequestRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<InvitationRequestExpirationJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public InvitationRequestExpirationJob(IInvitationRequestRepository invitationRequestRepository,
        IDateTimeProvider dateTimeProvider, ILogger<InvitationRequestExpirationJob> logger, IUnitOfWork unitOfWork)
    {
        _invitationRequestRepository = invitationRequestRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var updated = false;
        var results =
            await _invitationRequestRepository.GetAllToExpireAsync(_dateTimeProvider, SortOrder.Ascending,
                context.CancellationToken);

        if (results.IsError)
        {
            return;
        }

        foreach (var result in results.Value!)
        {
            result.Expire(_dateTimeProvider);
            await _invitationRequestRepository.UpdateAsync(result, context.CancellationToken);
            updated = true;
        }

        if (updated)
        {
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }
}