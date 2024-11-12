using Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationConf
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(typeof(ApplicationReference).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        return services;
    }
}