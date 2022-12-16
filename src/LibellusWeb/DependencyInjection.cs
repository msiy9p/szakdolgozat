using Libellus.Application.Common.Interfaces.Services;
using LibellusWeb.Common.RouteConstraints;
using LibellusWeb.Middleware;
using LibellusWeb.Services;

namespace LibellusWeb;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentPostService, CurrentPostService>();
        services.AddScoped<ICurrentGroupService, CurrentGroupService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<ExceptionHandlingMiddleware>();
        services.AddScoped<ClaimsPrincipalExtractor>();

        services.AddAntiforgery();
        services.AddRazorPages(options =>
            {
                // Routes
                options.Conventions.AddPageRoute("/", "/Login");

                // With Authorisation
                options.Conventions.AuthorizePage("/Logout");
                options.Conventions.AuthorizePage("/Welcome");

                options.Conventions.AuthorizeFolder("/Account");

                options.Conventions.AuthorizeFolder("/Group");

                options.Conventions.AuthorizeFolder("/Images");

                // With No Authorisation
                options.Conventions.AllowAnonymousToFolder("/Errors");
                options.Conventions.AllowAnonymousToFolder("/Pages");
            })
            .AddCookieTempDataProvider(options => { options.Cookie.IsEssential = true; });

        services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add(GroupIdConstraint.Key, typeof(GroupIdConstraint));
            options.ConstraintMap.Add(PostIdConstraint.Key, typeof(PostIdConstraint));
            options.ConstraintMap.Add(CommentIdConstraint.Key, typeof(CommentIdConstraint));
            options.ConstraintMap.Add(BookIdConstraint.Key, typeof(BookIdConstraint));
            options.ConstraintMap.Add(BookEditionIdConstraint.Key, typeof(BookEditionIdConstraint));
            options.ConstraintMap.Add(ShelfIdConstraint.Key, typeof(ShelfIdConstraint));
            options.ConstraintMap.Add(SeriesIdConstraint.Key, typeof(SeriesIdConstraint));
            options.ConstraintMap.Add(AuthorIdConstraint.Key, typeof(AuthorIdConstraint));
            options.ConstraintMap.Add(ReadingIdConstraint.Key, typeof(ReadingIdConstraint));

            options.ConstraintMap.Add(SearchTermConstraint.Key, typeof(SearchTermConstraint));
        });

        return services;
    }
}