using System.Threading.RateLimiting;
using Api.Endpoints;
using Application.Auth.Exceptions;
using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Serilog;

namespace Api;

public static class ApiConf
{
    public const string CORS_POLICY = "CorsPolicy";
    public const string OUTPUT_CACHE_POLICY = "OutputCachePolicy";
    public const string RATE_LIMITER_KEY = "GlobalLimiter";

    public static IServiceCollection AddApi(this WebApplicationBuilder builder)
    {
        ConfigureLoggin(builder);
        builder.Services.AddHttpContextAccessor();
        return builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .ConfigureRateLimiting()
            .ConfigureCors()
            .ConfigureOutputCache();
    }

    public static void ConfigureLoggin(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
    }

    public static void ConfigureOpenApi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(cfg =>
        {
            cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            cfg.RoutePrefix = "swagger";
        });
    }

    public static void ConfigureMiddlewareException(this WebApplication app)
    {
        app.UseExceptionHandler(cfg =>
        {
            cfg.Run(async ctx =>
            {
                var ctxFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                if (ctxFeature is not null)
                {
                    switch (ctxFeature.Error)
                    {
                        case ValidationException:
                            var exception = (ValidationException)ctxFeature.Error;
                            await Results.UnprocessableEntity(new ValidationProblemDetails(exception.Errors)).ExecuteAsync(ctx);
                            break;
                        case ForbiddenAccessException:
                            await Results.Forbid().ExecuteAsync(ctx);
                            break;
                        default:
                            await Results.Problem(detail: ctxFeature.Error.Message).ExecuteAsync(ctx);
                            break;
                    }
                }
            });
        });
    }

    public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(cfg =>
        {
            cfg.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                RateLimitPartition.GetFixedWindowLimiter(RATE_LIMITER_KEY, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 2,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(CORS_POLICY, cfg =>
            {
                cfg.AllowAnyOrigin();
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
            });
        });
        return services;
    }

    public static IServiceCollection ConfigureOutputCache(this IServiceCollection services)
    {
        services.AddOutputCache(cfg =>
        {
            cfg.AddPolicy(OUTPUT_CACHE_POLICY, builder =>
            {
                builder.Expire(TimeSpan.FromSeconds(5));
            });
        });
        return services;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGreetingEndpoints();
        app.MapAuthEndpoints();
        app.MapSwagger();
        app.MapScalarApiReference(opt => {
            opt.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
        });
    }
}