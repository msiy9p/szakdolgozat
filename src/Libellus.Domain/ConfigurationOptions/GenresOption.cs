using Libellus.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Libellus.Domain.ConfigurationOptions;

public sealed class GenresOption
{
    public const string OptionName = "Genres";

    public List<GenreItemOption> Items { get; set; } = new();
}

public sealed class GenreItemOption
{
    [Required] public string Name { get; set; } = string.Empty;

    [Required] public bool IsFiction { get; set; } = Genre.IsFictionDefault;
}