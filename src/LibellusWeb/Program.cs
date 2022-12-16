using Libellus.Application;
using Libellus.Domain;
using Libellus.Infrastructure;
using Libellus.Infrastructure.Persistence;
using LibellusWeb.Middleware;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;

namespace LibellusWeb;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("defaults.json", optional: true);

        builder.Services.AddDomain(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddPersistenceInfrastructure(builder.Configuration);
        builder.Services.AddWeb(builder.Configuration);

        builder.Host.UseSerilog((ctx, lc) => lc
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            await app.InitializeApplicationContextAsync();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                const int durationInSeconds = 60 * 60 * 24;
                ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                    "public,max-age=" + durationInSeconds;
            }
        });

        app.UseCookiePolicy();

        app.UseStatusCodePagesWithReExecute("/Errors/{0}");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}