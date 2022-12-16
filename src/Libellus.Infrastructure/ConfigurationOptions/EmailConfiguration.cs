namespace Libellus.Infrastructure.ConfigurationOptions;

public sealed class EmailConfiguration
{
    public const string OptionName = nameof(EmailConfiguration);

    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}