using Application.Auth;
using Application.UseCases.Greeting.Queries;
using Application.UseCases.Persons.Commands;
using Application.UseCases.Persons.Queries;
using Francisvac.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var personGroup = routeBuilder
            .MapGroup("persons")
            .WithOpenApi()
            .RequireCors(ApiConf.CORS_POLICY)
            .CacheOutput(ApiConf.OUTPUT_CACHE_POLICY)
            .RequireRateLimiting(ApiConf.RATE_LIMITER_KEY)
            .RequireAuthorization();
            
        personGroup.MapGet("/", async ([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? search, ISender sender) 
            => await sender.Send(new GetPaginatedPersonsQuery(pageNumber, pageSize, search ?? string.Empty)));
        personGroup.MapGet("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new GetPersonByIdQuery(id))).ToMinimalApiResult());
        personGroup.MapDelete("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new DeletePersonCommand(id))).ToMinimalApiResult());
        personGroup.MapPut("/{id}", async ([FromRoute] string id, [FromBody] UpdatePersonCommand command, ISender sender)
            => (await sender.Send(command with { Id = id})).ToMinimalApiResult());
        personGroup.MapPost("/", async ([FromBody] CreatePersonCommand command, ISender sender)
            => (await sender.Send(command)).ToMinimalApiResult());
    }
}