using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class LabelsOption
{
    public const string OptionName = "Labels";

    public List<LabelItemOption> Items { get; set; } = new();
}

public sealed class LabelItemOption
{
    [Required] public string Name { get; set; } = string.Empty;
}