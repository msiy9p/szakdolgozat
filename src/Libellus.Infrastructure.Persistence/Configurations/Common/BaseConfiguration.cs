using EFCore.NamingConventions.Internal;
using System.Globalization;

namespace Libellus.Infrastructure.Persistence.Configurations.Common;

internal abstract class BaseConfiguration
{
    internal const string DefaultSchemaName = "libellus";
    internal const string SocialSchemaName = "libellus_social";
    internal const string MediaSchemaName = "libellus_media";
    internal const string SecuritySchemaName = "libellus_security";
    internal const string HistorySchemaName = "libellus_history";
    internal const string SystemSchemaName = "libellus_system";
    internal const string EfCoreMigrationsTableName = "efcore_migrations_history";

    protected internal const int ShortNameLength = 50;
    protected internal const int NameLength = 250;
    protected internal const int TitleLength = 250;
    protected internal const int DescriptionLength = 500;
    protected internal const int CommentLength = 10000;
    protected internal const int ObjectNameLength = 1024;
    protected internal const string FtsIndexMethod = "GIN";
    protected internal const string JsonColumnType = "jsonb";

    private static readonly INameRewriter s_reWriter = new SnakeCaseNameRewriter(CultureInfo.CurrentCulture);
    private readonly Stepper _stepper = new();

    protected internal static string Rewrite(string input) => s_reWriter.RewriteName(input);

    protected int NextColumnOrder() => _stepper.Step();

    private sealed class Stepper
    {
        private int _current;

        public Stepper()
        {
            _current = -1;
        }

        public int Step() => _current++;
    }

    internal static class CustomSearchLanguageOne
    {
        private const string DefaultLanguage = "simple";
        private static string? _customLanguage = null;

        internal static bool SetCustomLanguage(string language)
        {
            if (_customLanguage is null && !string.IsNullOrWhiteSpace(language))
            {
                _customLanguage = language;

                return true;
            }

            return false;
        }

        internal static string GetLanguageName()
        {
            if (_customLanguage is not null)
            {
                return _customLanguage;
            }

            return DefaultLanguage;
        }
    }

    internal static class CustomSearchLanguageTwo
    {
        private const string DefaultLanguage = "english";
        private static string? _customLanguage = null;

        internal static bool SetCustomLanguage(string language)
        {
            if (_customLanguage is null && !string.IsNullOrWhiteSpace(language))
            {
                _customLanguage = language;

                return true;
            }

            return false;
        }

        internal static string GetLanguageName()
        {
            if (_customLanguage is not null)
            {
                return _customLanguage;
            }

            return DefaultLanguage;
        }
    }
}