using Application.UseCases.Persons.Queries;
using Application.UseCases.PositionBlueprints.Commands;
using Francisvac.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class PositionBlueprintEndpoints
{
    public static void MapPositionBlueprintEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var positionBlueprintGroup = routeBuilder
            .MapGroup("position-blueprints")
            .WithOpenApi()
            .RequireCors(ApiConf.CORS_POLICY)
            .CacheOutput(ApiConf.OUTPUT_CACHE_POLICY)
            .RequireRateLimiting(ApiConf.RATE_LIMITER_KEY)
            .RequireAuthorization();
            
        positionBlueprintGroup.MapGet("/", async ([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? search, ISender sender) 
            => await sender.Send(new GetPaginatedPositionBlueprintsQuery(pageNumber, pageSize, search ?? string.Empty)));
        positionBlueprintGroup.MapGet("/base-list", async (ISender sender) 
            => await sender.Send(new GetBaseListOfPositionBlueprintQuery()));
        positionBlueprintGroup.MapGet("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new GetPositionBluentprintByIdQuery(id))).ToMinimalApiResult());
        positionBlueprintGroup.MapDelete("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new DeletePositionBlueprintCommand(id))).ToMinimalApiResult());
        positionBlueprintGroup.MapPut("/{id}", async ([FromRoute] string id, [FromBody] UpdatePositionBlueprintCommand command, ISender sender)
            => (await sender.Send(command with { Id = id})).ToMinimalApiResult());
        positionBlueprintGroup.MapPost("/", async ([FromBody] CreatePositionBlueprintCommand command, ISender sender)
            => (await sender.Send(command)).ToMinimalApiResult());
    }
}