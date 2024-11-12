using Api.Endpoints;
using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Api;

public static class ApiConf
{
    public static IServiceCollection ConfigureApi(this WebApplicationBuilder builder)
    {
        ConfigureLoggin(builder);
        return builder.Services;
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
                        default:
                            await Results.Problem(detail: ctxFeature.Error.Message).ExecuteAsync(ctx);
                            break;
                    }
                }
            });
        });
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGreetingEndpoints();
    }
}