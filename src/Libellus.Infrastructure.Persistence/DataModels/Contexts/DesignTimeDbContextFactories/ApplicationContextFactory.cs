using Libellus.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Libellus.Infrastructure.Persistence.DataModels.Contexts.DesignTimeDbContextFactories;

internal sealed class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        // Get environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Build config
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<ApplicationContext>();

        options.UseNpgsql(config.GetConnectionString("PostgresConnection"), o =>
            {
                o.UseNodaTime()
                    .EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(1),
                        new List<string>())
                    .MigrationsHistoryTable(BaseConfiguration.EfCoreMigrationsTableName,
                        BaseConfiguration.HistorySchemaName);
            })
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention();

        return new ApplicationContext(options.Options);
    }
}