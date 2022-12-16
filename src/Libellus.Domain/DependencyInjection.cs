using Libellus.Domain.ConfigurationOptions;
using Libellus.Domain.DefaultEntities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Libellus.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration config)
    {
        config.ConfigureDomainDefaults();

        return services;
    }

    private static void ConfigureDomainDefaults(this IConfiguration config)
    {
        LoadFormatDefaults(config);
        LoadGenreDefaults(config);
        LoadLabelDefaults(config);
        LoadLanguageDefaults(config);
        LoadLiteratureFormDefaults(config);
        LoadPublisherDefaults(config);
        LoadTagDefaults(config);
        LoadWarningTagDefaults(config);
    }

    private static void LoadFormatDefaults(IConfiguration config)
    {
        var option = new FormatsOption();

        config.GetSection(FormatsOption.OptionName).Bind(option);

        DefaultFormats.ConfigureDefaults(option.Items);
    }

    private static void LoadGenreDefaults(IConfiguration config)
    {
        var option = new GenresOption();

        config.GetSection(GenresOption.OptionName).Bind(option);

        DefaultGenres.ConfigureDefaults(option.Items);
    }

    private static void LoadLabelDefaults(IConfiguration config)
    {
        var option = new LabelsOption();

        config.GetSection(LabelsOption.OptionName).Bind(option);

        DefaultLabels.ConfigureDefaults(option.Items);
    }

    private static void LoadLanguageDefaults(IConfiguration config)
    {
        var option = new LanguagesOption();

        config.GetSection(LanguagesOption.OptionName).Bind(option);

        DefaultLanguages.ConfigureDefaults(option.Items);
    }

    private static void LoadLiteratureFormDefaults(IConfiguration config)
    {
        var option = new LiteratureFormsOption();

        config.GetSection(LiteratureFormsOption.OptionName).Bind(option);

        DefaultLiteratureForms.ConfigureDefaults(option.Items);
    }

    private static void LoadPublisherDefaults(IConfiguration config)
    {
        var option = new PublishersOption();

        config.GetSection(PublishersOption.OptionName).Bind(option);

        DefaultPublishers.ConfigureDefaults(option.Items);
    }

    private static void LoadTagDefaults(IConfiguration config)
    {
        var option = new TagsOption();

        config.GetSection(TagsOption.OptionName).Bind(option);

        DefaultTags.ConfigureDefaults(option.Items);
    }

    private static void LoadWarningTagDefaults(IConfiguration config)
    {
        var option = new WarningTagsOption();

        config.GetSection(WarningTagsOption.OptionName).Bind(option);

        DefaultWarningTags.ConfigureDefaults(option.Items);
    }
}