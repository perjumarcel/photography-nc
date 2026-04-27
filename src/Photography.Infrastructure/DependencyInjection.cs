using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Photography.Core.Categories;
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

        var provider = configuration["Storage:Provider"] ?? "Local";
        services.Configure<LocalStorageOptions>(configuration.GetSection(LocalStorageOptions.SectionName));
        services.Configure<R2StorageOptions>(configuration.GetSection(R2StorageOptions.SectionName));

        if (string.Equals(provider, "R2", StringComparison.OrdinalIgnoreCase))
            services.AddSingleton<IStorageService, R2StorageService>();
        else
            services.AddSingleton<IStorageService, LocalFileSystemStorageService>();

        return services;
    }
}
