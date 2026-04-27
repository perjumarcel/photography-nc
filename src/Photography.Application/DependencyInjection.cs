using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Photography.Application.Common.Behaviors;
using Photography.Application.Common.Time;

namespace Photography.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPhotographyApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
