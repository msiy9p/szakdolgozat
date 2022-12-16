using System.ComponentModel.DataAnnotations;

namespace Libellus.Infrastructure.Persistence.ConfigurationOptions;

public sealed class CustomSearchLanguageOption
{
    public const string OptionName = nameof(CustomSearchLanguageOption);

    [Required] public string LanguageOne { get; set; } = "simple";

    [Required] public string LanguageTwo { get; set; } = "english";
}