using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Infrastructure.ConfigurationOptions;
using Libellus.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Libellus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddEmailService(config);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IImageResizerWithPreference, ImageSharpImageResizer>()
            .AddTransient<IImageResizer, ImageSharpImageResizer>();

        services.AddTransient<IEmailService, FluentEmailService>()
            .AddTransient<IBulkEmailService, BulkFluentEmailService>();

        services.AddTransient<IHtmlSanitizer, HtmlSanitizerWrapper>();

        return services;
    }

    private static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration config)
    {
        var emailConf = new EmailConfiguration();
        config.GetSection(EmailConfiguration.OptionName).Bind(emailConf);

        if (string.IsNullOrWhiteSpace(emailConf.FromEmail))
        {
            throw new ArgumentException("Invalid from email address.", nameof(emailConf.FromEmail));
        }

        if (string.IsNullOrWhiteSpace(emailConf.FromName))
        {
            throw new ArgumentException("Invalid from name.", nameof(emailConf.FromName));
        }

        var configured = false;

        if (!configured)
        {
            var smtpConf = new SmtpConfiguration();
            config.GetSection(SmtpConfiguration.OptionName).Bind(smtpConf);
            if (smtpConf.IsValid())
            {
                if (string.IsNullOrWhiteSpace(smtpConf.Username) || string.IsNullOrWhiteSpace(smtpConf.Password))
                {
                    services.AddFluentEmail(emailConf.FromEmail, emailConf.FromName)
                        .AddLiquidRenderer()
                        .AddSmtpSender(smtpConf.Host, smtpConf.Port);
                }
                else
                {
                    services.AddFluentEmail(emailConf.FromEmail, emailConf.FromName)
                        .AddLiquidRenderer()
                        .AddSmtpSender(smtpConf.Host, smtpConf.Port, smtpConf.Username, smtpConf.Password);
                }

                configured = true;
            }
        }

        if (!configured)
        {
            throw new ArgumentException("No email service configured.");
        }

        return services;
    }
}