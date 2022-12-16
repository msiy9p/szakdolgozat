using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class TagsOption
{
    public const string OptionName = "Tags";

    public List<TagItemOption> Items { get; set; } = new();
}

public sealed class TagItemOption
{
    [Required] public string Name { get; set; } = string.Empty;
}