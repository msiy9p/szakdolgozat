using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class PublishersOption
{
    public const string OptionName = "Publishers";

    public List<PublisherItemOption> Items { get; set; } = new();
}

public sealed class PublisherItemOption
{
    [Required] public string Name { get; set; } = string.Empty;
}