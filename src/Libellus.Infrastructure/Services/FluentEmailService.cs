using Ardalis.GuardClauses;
using FluentEmail.Core;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.EmailTemplates;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Services;

internal sealed class FluentEmailService : IEmailService
{
    private readonly ILogger<FluentEmailService> _logger;
    private readonly IFluentEmail _fluentEmail;

    public FluentEmailService(ILogger<FluentEmailService> logger, IFluentEmail fluentEmail)
    {
        _logger = Guard.Against.Null(logger);
        _fluentEmail = Guard.Against.Null(fluentEmail);
    }

    public async Task<Result> SendAsync(IEmailData emailData, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(emailData);

        var emailResult = await _fluentEmail
            .To(emailData.UserEmail, emailData.Username)
            .Subject(emailData.Subject)
            .SendAsync(cancellationToken);

        return emailResult.Successful ? Result.Success() : Result.Error();
    }

    public async Task<Result> SendInvitationAsync(InvitationVm invitationVm,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(invitationVm);

        var emailResult = await _fluentEmail
            .To(invitationVm.InviteeEmail, invitationVm.InviteeName)
            .Subject("New group invitation!")
            .UsingTemplateFromEmbedded(EmailTemplateHelper.InvitationTemplate, new
            {
                InviteeName = invitationVm.InviteeName,
                InviterName = invitationVm.InviterName,
                GroupName = invitationVm.GroupName,
            }, EmailTemplateHelper.Assembly)
            .HighPriority()
            .SendAsync(cancellationToken);

        return emailResult.Successful ? Result.Success() : Result.Error();
    }

    public async Task<Result> SendEmailConfirmationAsync(UserEmailVm userEmailVm, string emailConfirmationUrl,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(userEmailVm);
        Guard.Against.NullOrWhiteSpace(emailConfirmationUrl);

        var emailResult = await _fluentEmail
            .To(userEmailVm.UserEmail, userEmailVm.UserName)
            .Subject("Email confirmation.")
            .UsingTemplateFromEmbedded(EmailTemplateHelper.EmailConfirmationTemplate, new
            {
                Name = userEmailVm.UserName,
                EmailConfirmationUrl = emailConfirmationUrl,
            }, EmailTemplateHelper.Assembly)
            .HighPriority()
            .SendAsync(cancellationToken);

        return emailResult.Successful ? Result.Success() : Result.Error();
    }

    public async Task<Result> SendChangeEmailTokenAsync(UserEmailVm userEmailVm, string changeEmailUrl,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(userEmailVm);
        Guard.Against.NullOrWhiteSpace(changeEmailUrl);

        var emailResult = await _fluentEmail
            .To(userEmailVm.UserEmail, userEmailVm.UserName)
            .Subject("Email change.")
            .UsingTemplateFromEmbedded(EmailTemplateHelper.ChangeEmailTemplate, new
            {
                Name = userEmailVm.UserName,
                ChangeEmailUrl = changeEmailUrl,
            }, EmailTemplateHelper.Assembly)
            .HighPriority()
            .SendAsync(cancellationToken);

        return emailResult.Successful ? Result.Success() : Result.Error();
    }

    public async Task<Result> SendResetPasswordTokenAsync(UserEmailVm userEmailVm, string resetTokenUrl,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(userEmailVm);
        Guard.Against.NullOrWhiteSpace(resetTokenUrl);

        var emailResult = await _fluentEmail
            .To(userEmailVm.UserEmail, userEmailVm.UserName)
            .Subject("Password reset.")
            .UsingTemplateFromEmbedded(EmailTemplateHelper.ResetPasswordTemplate, new
            {
                Name = userEmailVm.UserName,
                ResetTokenUrl = resetTokenUrl,
            }, EmailTemplateHelper.Assembly)
            .HighPriority()
            .SendAsync(cancellationToken);

        return emailResult.Successful ? Result.Success() : Result.Error();
    }
}