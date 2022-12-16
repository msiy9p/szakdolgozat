using System.ComponentModel.DataAnnotations;
using SM = Libellus.Domain.Common.Types.ScoreMultiplier;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class LiteratureFormsOption
{
    public const string OptionName = "LiteratureForms";

    public List<LiteratureFormItemOption> Items { get; set; } = new();
}

public sealed class LiteratureFormItemOption
{
    [Required] public string Name { get; set; } = string.Empty;

    [Required] public decimal ScoreMultiplier { get; set; } = SM.ScoreMultiplierDefault;
}