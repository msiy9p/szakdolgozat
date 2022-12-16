using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class LanguagesOption
{
    public const string OptionName = "Languages";

    public List<LanguageItemOption> Items { get; set; } = new();
}

public sealed class LanguageItemOption
{
    [Required] public string Name { get; set; } = string.Empty;
}