using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class WarningTagsOption
{
    public const string OptionName = "WarningTags";

    public List<WarningTagItemOption> Items { get; set; } = new();
}

public sealed class WarningTagItemOption
{
    [Required] public string Name { get; set; } = string.Empty;
}