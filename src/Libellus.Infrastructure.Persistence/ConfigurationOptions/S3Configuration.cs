using System.ComponentModel.DataAnnotations;

namespace Libellus.Infrastructure.Persistence.ConfigurationOptions;

public sealed class S3Configuration
{
    public const string OptionName = nameof(S3Configuration);

    [Required] public string Endpoint { get; set; } = string.Empty;

    [Required] public string AccessKey { get; set; } = string.Empty;

    [Required] public string SecretKey { get; set; } = string.Empty;

    [Required] public string Region { get; set; } = string.Empty;

    public bool WithSsl { get; set; } = false;
}