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
            .MapGroup("greeting");
            
        greetingGroup.MapGet("/", async ([FromQuery] string name, ISender sender) 
            => (await sender.Send(new GreetingQuery(name))).ToMinimalApiResult());
    }
}