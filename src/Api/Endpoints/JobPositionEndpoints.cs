using Application.UseCases.JobPositions.Commands;
using Application.UseCases.JobPositions.Queries;
using Francisvac.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class JobPositionEndpoints
{
    public static void MapJobPositionEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var jobPositionGroup = routeBuilder
            .MapGroup("job-positions")
            .WithOpenApi()
            .RequireCors(ApiConf.CORS_POLICY)
            .CacheOutput(ApiConf.OUTPUT_CACHE_POLICY)
            .RequireRateLimiting(ApiConf.RATE_LIMITER_KEY)
            .RequireAuthorization();
            
        jobPositionGroup.MapGet("/", async ([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? search, [FromQuery] string? positionBlueprintId, [FromQuery] bool? isVacant, ISender sender) 
            => await sender.Send(new GetPaginatedJobPositionsQuery(pageNumber, pageSize, search ?? string.Empty, positionBlueprintId ?? string.Empty, isVacant)));
        jobPositionGroup.MapGet("/base-list", async (ISender sender) 
            => await sender.Send(new GetBaseListOfJobPositionQuery()));
        jobPositionGroup.MapGet("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new GetJobPositionByIdQuery(id))).ToMinimalApiResult());
        jobPositionGroup.MapDelete("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new DeleteJobPositionCommand(id))).ToMinimalApiResult());
        jobPositionGroup.MapPost("/", async ([FromBody] CreateJobPositionCommand command, ISender sender)
            => (await sender.Send(command)).ToMinimalApiResult());
    }
}