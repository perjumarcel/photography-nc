using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Photography.Application.Common.Email;
using Photography.Application.Common.Imaging;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.Core.Users;
using Photography.Infrastructure.Email;
using Photography.Infrastructure.Imaging;
using Photography.Infrastructure.Persistence;
using Photography.Infrastructure.Persistence.Repositories;
using Photography.Infrastructure.Storage;

namespace Photography.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPhotographyInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            var cs = configuration.GetConnectionString("Default")
                     ?? throw new InvalidOperationException("ConnectionStrings:Default is required");
            opts.UseNpgsql(cs, npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        });

        services.AddScoped<IAlbumQueryRepository, AlbumQueryRepository>();
        services.AddScoped<IAlbumCommandRepository, AlbumCommandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IImageMetadataReader, ImageSharpMetadataReader>();

        var storageProvider = configuration["Storage:Provider"] ?? "Local";
        services.Configure<LocalStorageOptions>(configuration.GetSection(LocalStorageOptions.SectionName));
        services.Configure<R2StorageOptions>(configuration.GetSection(R2StorageOptions.SectionName));

        if (string.Equals(storageProvider, "R2", StringComparison.OrdinalIgnoreCase))
            services.AddSingleton<IStorageService, R2StorageService>();
        else
            services.AddSingleton<IStorageService, LocalFileSystemStorageService>();

        // ── Email ──────────────────────────────────────────────────────────
        // Same provider-switch pattern as Storage. SMTP defaults to no-op so
        // that local/CI environments work without credentials.
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        var emailProvider = configuration[$"{EmailOptions.SectionName}:Provider"] ?? "NoOp";
        if (string.Equals(emailProvider, "Smtp", StringComparison.OrdinalIgnoreCase))
            services.AddTransient<IEmailSender, SmtpEmailSender>();
        else
            services.AddSingleton<IEmailSender, NoOpEmailSender>();

        return services;
    }
}
