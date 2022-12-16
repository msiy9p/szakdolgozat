namespace Libellus.Infrastructure.ConfigurationOptions;

public sealed class SmtpConfiguration
{
    public const string OptionName = nameof(SmtpConfiguration);

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool IsValid()
    {
        return true;
        //return !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Username) &&
        //       !string.IsNullOrWhiteSpace(Password) && Port > 0;
    }
}