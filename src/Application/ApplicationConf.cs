using Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationConf
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        var applicationAsem = typeof(ApplicationReference).Assembly;
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(applicationAsem);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(applicationAsem);
        return services;
    }
}