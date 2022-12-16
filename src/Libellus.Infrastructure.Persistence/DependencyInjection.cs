using Ardalis.GuardClauses;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.BackgroundJobs;
using Libellus.Infrastructure.Persistence.ConfigurationOptions;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.Configurations.ValueConverters;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.DataModels.Contexts.Initializers;
using Libellus.Infrastructure.Persistence.DataModels.Contexts.Interceptors;
using Libellus.Infrastructure.Persistence.DataModels.Contexts.Stores;
using Libellus.Infrastructure.Persistence.Repositories;
using Libellus.Infrastructure.Persistence.Repositories.Cached;
using Libellus.Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio.AspNetCore;
using Quartz;

namespace Libellus.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSetCustomSearchLanguage(config);

        // Storage
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
        {
            options.AddInterceptors(serviceProvider.GetRequiredService<DomainEventDispatchInterceptor>());

            // TODO: Tracing?
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
        });

        services.AddMinio(options =>
        {
            var conf = new S3Configuration();
            config.GetSection(S3Configuration.OptionName).Bind(conf);

            options.Endpoint = conf.Endpoint;
            options.AccessKey = conf.AccessKey;
            options.SecretKey = conf.SecretKey;
            options.Region = conf.Region;

            if (conf.WithSsl)
            {
                options.ConfigureClient(client => { client.WithSSL(); });
            }
        });

        services.AddStackExchangeRedisCache(x => { x.Configuration = config.GetConnectionString("RedisConnection"); });

        // Repo
        services.AddRepositories(config);

        //  Auth
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;

            options.MinimumSameSitePolicy = SameSiteMode.Strict;

            options.HttpOnly = HttpOnlyPolicy.Always;
        });

        services.AddAuthentication();
        services.AddAuthorizationCore();

        services.AddScoped<IUserStore<ApplicationUser>, ApplicationUserStore>();
        services.AddScoped<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
        services.AddScoped<IPasswordHasher<ApplicationUser>, CustomPasswordHasher>();
        services.AddScoped<IPasswordValidator<ApplicationUser>, CustomPasswordValidator>();

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 1;
            options.Password.RequiredLength = 8;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            options.User.RequireUniqueEmail = true;
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = "/Errors/AccessDenied";
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
        });

        services.AddScoped<ApplicationContextInitializer>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Background jobs
        services.AddQuartz(c =>
        {
            c.AddJobs();

            c.UseMicrosoftDependencyInjectionJobFactory();
        });
        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

        return services;
    }

    private static IServiceCollection AddSetCustomSearchLanguage(this IServiceCollection services,
        IConfiguration config)
    {
        Guard.Against.Null(services);

        var conf = new CustomSearchLanguageOption();
        config.GetSection(CustomSearchLanguageOption.OptionName).Bind(conf);

        BaseConfiguration.CustomSearchLanguageOne.SetCustomLanguage(conf.LanguageOne);
        BaseConfiguration.CustomSearchLanguageTwo.SetCustomLanguage(conf.LanguageTwo);

        return services;
    }

    private static IServiceCollectionQuartzConfigurator AddJobs(this IServiceCollectionQuartzConfigurator options)
    {
        Guard.Against.Null(options);

        options.AddJob<InvitationExpirationJob>(InvitationExpirationJob.JobKey)
            .AddTrigger(t => t.ForJob(InvitationExpirationJob.JobKey)
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever()));

        options.AddJob<InvitationRequestExpirationJob>(InvitationRequestExpirationJob.JobKey)
            .AddTrigger(t => t.ForJob(InvitationRequestExpirationJob.JobKey)
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever()));

        options.AddJob<ReadingExpirationJob>(ReadingExpirationJob.JobKey)
            .AddTrigger(t => t.ForJob(ReadingExpirationJob.JobKey)
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(12)
                    .RepeatForever()));

        options.AddJob<BookEditionReleasingJob>(BookEditionReleasingJob.JobKey)
            .AddTrigger(t => t.ForJob(BookEditionReleasingJob.JobKey)
                .WithCronSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 0)));

        options.AddJob<PostLockJob>(PostLockJob.JobKey)
            .AddTrigger(t => t.ForJob(PostLockJob.JobKey)
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever()));

        return options;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        Guard.Against.Null(services);

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var useCache = false;
        if (bool.TryParse(config["UseCache"], out var result))
        {
            useCache = result;
        }

        if (useCache)
        {
            services.AddScoped<ICoverImageReadOnlyRepository, CoverImageRepository>()
                .AddScoped<ICoverImageRepository, CoverImageRepository>();

            services.AddScoped<ICoverImageMetaDataReadOnlyRepository, CoverImageMetadataRepository>()
                .AddScoped<ICoverImageMetaDataRepository, CoverImageMetadataRepository>();

            services.AddScoped<IProfilePictureReadOnlyRepository, ProfilePictureRepository>()
                .AddScoped<IProfilePictureRepository, ProfilePictureRepository>();

            services.AddScoped<IProfilePictureMetaDataReadOnlyRepository, ProfilePictureMetadataRepository>()
                .AddScoped<IProfilePictureMetaDataRepository, ProfilePictureMetadataRepository>();

            services.AddScoped<IAuthorReadOnlyRepository, AuthorRepository>()
                .AddScoped<IAuthorRepository, AuthorRepository>();

            services.AddScoped<IBookEditionReadOnlyRepository, BookEditionRepository>()
                .AddScoped<IBookEditionRepository, BookEditionRepository>();

            services.AddScoped<IBookEditionReleasingRepository, BookEditionReleasingRepository>();

            services.AddScoped<IBookReadOnlyRepository, BookRepository>()
                .AddScoped<IBookRepository, BookRepository>();

            services.AddScoped<ICommentReadOnlyRepository, CommentRepository>()
                .AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IFormatReadOnlyRepository, FormatRepository>()
                .AddScoped<IFormatRepository, FormatRepository>();

            services.AddScoped<IGenreReadOnlyRepository, GenreRepository>()
                .AddScoped<IGenreRepository, GenreRepository>();

            services.AddScoped<IGroupReadOnlyRepository, GroupRepository>()
                .AddScoped<IGroupRepository, GroupRepository>();

            services.AddScoped<IGroupMembershipReadOnlyRepository, GroupMembershipRepository>()
                .AddScoped<IGroupMembershipRepository, GroupMembershipRepository>();

            services.AddScoped<IInvitationReadOnlyRepository, InvitationRepository>()
                .AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<IInvitationRequestReadOnlyRepository, InvitationRequestRepository>()
                .AddScoped<IInvitationRequestRepository, InvitationRequestRepository>();

            services.AddScoped<ILabelReadOnlyRepository, LabelRepository>()
                .AddScoped<ILabelRepository, LabelRepository>();

            services.AddScoped<ILanguageReadOnlyRepository, LanguageRepository>()
                .AddScoped<ILanguageRepository, LanguageRepository>();

            services.AddScoped<ILiteratureFormReadOnlyRepository, LiteratureFormRepository>()
                .AddScoped<ILiteratureFormRepository, LiteratureFormRepository>();

            services.AddScoped<INoteReadOnlyRepository, NoteRepository>()
                .AddScoped<INoteRepository, NoteRepository>();

            services.AddScoped<IPostReadOnlyRepository, PostRepository>()
                .AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IPublisherReadOnlyRepository, PublisherRepository>()
                .AddScoped<IPublisherRepository, PublisherRepository>();

            services.AddScoped<IReadingReadOnlyRepository, ReadingRepository>()
                .AddScoped<IReadingRepository, ReadingRepository>();

            services.AddScoped<ISeriesReadOnlyRepository, SeriesRepository>()
                .AddScoped<ISeriesRepository, SeriesRepository>();

            services.AddScoped<IShelfReadOnlyRepository, ShelfRepository>()
                .AddScoped<IShelfRepository, ShelfRepository>();

            services.AddScoped<ITagReadOnlyRepository, TagRepository>()
                .AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<IWarningTagReadOnlyRepository, WarningTagRepository>()
                .AddScoped<IWarningTagRepository, WarningTagRepository>();

            services.AddScoped<IUserReadOnlyRepository, UserRepository>()
                .AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<FriendlyIdLookupRepository>();
            services.AddScoped<IFriendlyIdLookupRepository, CachedFriendlyIdLookupRepository>();
        }
        else
        {
            services.AddScoped<ICoverImageReadOnlyRepository, CoverImageRepository>()
                .AddScoped<ICoverImageRepository, CoverImageRepository>();

            services.AddScoped<ICoverImageMetaDataReadOnlyRepository, CoverImageMetadataRepository>()
                .AddScoped<ICoverImageMetaDataRepository, CoverImageMetadataRepository>();

            services.AddScoped<IProfilePictureReadOnlyRepository, ProfilePictureRepository>()
                .AddScoped<IProfilePictureRepository, ProfilePictureRepository>();

            services.AddScoped<IProfilePictureMetaDataReadOnlyRepository, ProfilePictureMetadataRepository>()
                .AddScoped<IProfilePictureMetaDataRepository, ProfilePictureMetadataRepository>();

            services.AddScoped<IAuthorReadOnlyRepository, AuthorRepository>()
                .AddScoped<IAuthorRepository, AuthorRepository>();

            services.AddScoped<IBookEditionReadOnlyRepository, BookEditionRepository>()
                .AddScoped<IBookEditionRepository, BookEditionRepository>();

            services.AddScoped<IBookEditionReleasingRepository, BookEditionReleasingRepository>();

            services.AddScoped<IBookReadOnlyRepository, BookRepository>()
                .AddScoped<IBookRepository, BookRepository>();

            services.AddScoped<ICommentReadOnlyRepository, CommentRepository>()
                .AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IFormatReadOnlyRepository, FormatRepository>()
                .AddScoped<IFormatRepository, FormatRepository>();

            services.AddScoped<IGenreReadOnlyRepository, GenreRepository>()
                .AddScoped<IGenreRepository, GenreRepository>();

            services.AddScoped<IGroupReadOnlyRepository, GroupRepository>()
                .AddScoped<IGroupRepository, GroupRepository>();

            services.AddScoped<IGroupMembershipReadOnlyRepository, GroupMembershipRepository>()
                .AddScoped<IGroupMembershipRepository, GroupMembershipRepository>();

            services.AddScoped<IInvitationReadOnlyRepository, InvitationRepository>()
                .AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<IInvitationRequestReadOnlyRepository, InvitationRequestRepository>()
                .AddScoped<IInvitationRequestRepository, InvitationRequestRepository>();

            services.AddScoped<ILabelReadOnlyRepository, LabelRepository>()
                .AddScoped<ILabelRepository, LabelRepository>();

            services.AddScoped<ILanguageReadOnlyRepository, LanguageRepository>()
                .AddScoped<ILanguageRepository, LanguageRepository>();

            services.AddScoped<ILiteratureFormReadOnlyRepository, LiteratureFormRepository>()
                .AddScoped<ILiteratureFormRepository, LiteratureFormRepository>();

            services.AddScoped<INoteReadOnlyRepository, NoteRepository>()
                .AddScoped<INoteRepository, NoteRepository>();

            services.AddScoped<IPostReadOnlyRepository, PostRepository>()
                .AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IPublisherReadOnlyRepository, PublisherRepository>()
                .AddScoped<IPublisherRepository, PublisherRepository>();

            services.AddScoped<IReadingReadOnlyRepository, ReadingRepository>()
                .AddScoped<IReadingRepository, ReadingRepository>();

            services.AddScoped<ISeriesReadOnlyRepository, SeriesRepository>()
                .AddScoped<ISeriesRepository, SeriesRepository>();

            services.AddScoped<IShelfReadOnlyRepository, ShelfRepository>()
                .AddScoped<IShelfRepository, ShelfRepository>();

            services.AddScoped<ITagReadOnlyRepository, TagRepository>()
                .AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<IWarningTagReadOnlyRepository, WarningTagRepository>()
                .AddScoped<IWarningTagRepository, WarningTagRepository>();

            services.AddScoped<IUserReadOnlyRepository, UserRepository>()
                .AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IFriendlyIdLookupRepository, FriendlyIdLookupRepository>();
        }

        return services;
    }

    public static ModelConfigurationBuilder AddIdConversions(this ModelConfigurationBuilder configurationBuilder)
    {
        Guard.Against.Null(configurationBuilder);

        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();

        configurationBuilder.Properties<CoverImageId>().HaveConversion<CoverImageIdConverter>();
        configurationBuilder.Properties<ProfilePictureId>().HaveConversion<ProfilePictureIdConverter>();

        configurationBuilder.Properties<CommentId>().HaveConversion<CommentIdConverter>();
        configurationBuilder.Properties<GroupId>().HaveConversion<GroupIdConverter>();
        configurationBuilder.Properties<LabelId>().HaveConversion<LabelIdConverter>();
        configurationBuilder.Properties<PostId>().HaveConversion<PostIdConverter>();

        configurationBuilder.Properties<AuthorId>().HaveConversion<AuthorIdConverter>();
        configurationBuilder.Properties<BookEditionId>().HaveConversion<BookEditionIdConverter>();
        configurationBuilder.Properties<BookId>().HaveConversion<BookIdConverter>();
        configurationBuilder.Properties<FormatId>().HaveConversion<FormatIdConverter>();
        configurationBuilder.Properties<GenreId>().HaveConversion<GenreIdConverter>();
        configurationBuilder.Properties<LanguageId>().HaveConversion<LanguageIdConverter>();
        configurationBuilder.Properties<LiteratureFormId>().HaveConversion<LiteratureFormIdConverter>();
        configurationBuilder.Properties<NoteId>().HaveConversion<NoteIdConverter>();
        configurationBuilder.Properties<PublisherId>().HaveConversion<PublisherIdConverter>();
        configurationBuilder.Properties<ReadingId>().HaveConversion<ReadingIdConverter>();
        configurationBuilder.Properties<SeriesId>().HaveConversion<SeriesIdConverter>();
        configurationBuilder.Properties<ShelfId>().HaveConversion<ShelfIdConverter>();
        configurationBuilder.Properties<TagId>().HaveConversion<TagIdConverter>();
        configurationBuilder.Properties<WarningTagId>().HaveConversion<WarningTagIdConverter>();

        configurationBuilder.Properties<InvitationId>().HaveConversion<InvitationIdConverter>();

        return configurationBuilder;
    }

    public static async Task<IApplicationBuilder> InitializeApplicationContextAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationContextInitializer>();
            await initializer.InitialiseAsync();
            await initializer.SeedAsync();
        }

        return app;
    }
}