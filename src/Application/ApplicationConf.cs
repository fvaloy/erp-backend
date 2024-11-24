using Application.Auth;
using Application.Auth.Jwt;
using Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationConf
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAsem = typeof(ApplicationReference).Assembly;
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(applicationAsem);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
        });
        services.AddScoped<TokenManager>();
        services.ConfigureOptions<JWTOptionsSetup>();
        services.AddValidatorsFromAssembly(applicationAsem);
        return services;
    }
}