using Application.UseCases.Auth.Commands;
using Francisvac.Result.AspNetCore;
using MediatR;

namespace Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var authGroup = routeBuilder
            .MapGroup("auth")
            .WithOpenApi()
            .RequireCors(ApiConf.CORS_POLICY)
            .CacheOutput(ApiConf.OUTPUT_CACHE_POLICY)
            .RequireRateLimiting(ApiConf.RATE_LIMITER_KEY);
        
        authGroup.MapPost("/register", async (RegisterCommand command, ISender sender) => (await sender.Send(command)).ToMinimalApiResult());
        authGroup.MapPost("/login", async (LoginCommand command, ISender sender) => (await sender.Send(command)).ToMinimalApiResult());
        authGroup.MapPost("/refresh-token", async (RefreshTokenCommand command, ISender sender) => (await sender.Send(command)).ToMinimalApiResult());
    }
}