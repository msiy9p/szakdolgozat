using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Text;

namespace Libellus.Infrastructure.Persistence.Utilities;

internal static class Extensions
{
    public static string Base64UrlEncode(string value)
    {
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(value));
    }

    public static string Base64UrlDecode(string value)
    {
        return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(value));
    }

    public static ModelConfigurationBuilder AddIdConversionsFromAssembly(
        this ModelConfigurationBuilder configurationBuilder, Assembly assembly)
    {
        Guard.Against.Null(configurationBuilder);

        if (assembly is null)
        {
            return configurationBuilder;
        }

        foreach (var type in assembly.GetTypes()
                     .Where(x => x.IsSubclassOf(typeof(ValueConverter<,>)))
                     .Where(x => x.IsSealed)
                     .Where(x => x.Name.EndsWith("IdConverter")))
        {
            var types = type.GetGenericArguments();
            if (types.Length != 2)
            {
                continue;
            }

            if (types[0].GetInterface(nameof(ICustomIdType<Guid>)) is null || types[1] != typeof(Guid))
            {
                continue;
            }

            configurationBuilder.Properties(types[0]).HaveConversion(type);
        }

        return configurationBuilder;
    }
}