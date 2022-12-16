using Libellus.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class FormatsOption
{
    public const string OptionName = "Formats";

    public List<FormatItemOption> Items { get; set; } = new();
}

public sealed class FormatItemOption
{
    [Required] public string Name { get; set; } = string.Empty;

    [Required] public bool IsDigital { get; set; } = Format.IsDigitalDefault;
}