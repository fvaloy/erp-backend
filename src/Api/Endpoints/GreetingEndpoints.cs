using Application.Auth;
using Application.UseCases.Greeting.Queries;
using Francisvac.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class GreetingEndpoints
{
    public static void MapGreetingEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var greetingGroup = routeBuilder
            .MapGroup("greeting")
            .WithOpenApi()
            .RequireCors(ApiConf.CORS_POLICY)
            .CacheOutput(ApiConf.OUTPUT_CACHE_POLICY)
            .RequireRateLimiting(ApiConf.RATE_LIMITER_KEY)
            .RequireAuthorization();
            
        greetingGroup.MapGet("/", async ([FromQuery] string name, ISender sender) 
            => (await sender.Send(new GreetingQuery(name))).ToMinimalApiResult());
    }
}