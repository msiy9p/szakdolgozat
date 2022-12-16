using Ardalis.GuardClauses;
using FluentEmail.Core;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Libellus.Infrastructure.EmailTemplates;

namespace Libellus.Infrastructure.Services;

internal sealed class BulkFluentEmailService : IBulkEmailService
{
    private readonly ILogger<BulkFluentEmailService> _logger;
    private readonly IFluentEmailFactory _fluentEmailFactory;

    public BulkFluentEmailService(ILogger<BulkFluentEmailService> logger, IFluentEmailFactory fluentEmailFactory)
    {
        _logger = Guard.Against.Null(logger);
        _fluentEmailFactory = Guard.Against.Null(fluentEmailFactory);
    }

    public async Task<IReadOnlyList<Result>> SendAsync(IReadOnlyList<IEmailData> emailData,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(emailData);

        var results = new List<Result>(emailData.Count);

        foreach (var emailD in emailData)
        {
            var emailResult = await _fluentEmailFactory.Create()
                .To(emailD.UserEmail, emailD.Username)
                .Subject(emailD.Subject)
                .SendAsync(cancellationToken);

            results.Add(emailResult.Successful ? Result.Success() : Result.Error());
        }

        return results;
    }

    public async Task<Result> SendAsync(BookEditionReleasingVm bookEditionReleasing,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(bookEditionReleasing);

        var results = new List<Result>(bookEditionReleasing.Users.Count);

        foreach (var user in bookEditionReleasing.Users)
        {
            var emailResult = await _fluentEmailFactory.Create()
                .To(user.UserEmail, user.UserName)
                .Subject("Book release near!")
                .UsingTemplateFromEmbedded(EmailTemplateHelper.BookEditionReleasingTemplate, new
                {
                    Name = user.UserName,
                    Title = bookEditionReleasing.BookEditionTitle,
                    Date = bookEditionReleasing.ReleaseDate.ToString("d", DateTimeFormatInfo.CurrentInfo)
                }, EmailTemplateHelper.Assembly)
                .LowPriority()
                .SendAsync(cancellationToken);

            results.Add(emailResult.Successful ? Result.Success() : Result.Error());
        }

        return results.Count > 0 ? Result.Error() : Result.Success();
    }
}