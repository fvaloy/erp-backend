using Application.UseCases.Employees.Commands;
using Application.UseCases.Employees.Queries;
using Application.UseCases.Persons.Queries;
using Francisvac.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var employeeGroup = routeBuilder
            .MapGroup("employees")
            .WithOpenApi()
            .RequireCors(ApiConf.CORS_POLICY)
            .CacheOutput(ApiConf.OUTPUT_CACHE_POLICY)
            .RequireRateLimiting(ApiConf.RATE_LIMITER_KEY)
            .RequireAuthorization();
            
        employeeGroup.MapGet("/", async ([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? search, [FromQuery] string? positionBlueprintId, ISender sender) 
            => await sender.Send(new GetPaginatedEmployeeQuery(pageNumber, pageSize, search ?? string.Empty, positionBlueprintId ?? string.Empty)));
        employeeGroup.MapGet("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new GetEmployeeByIdQuery(id))).ToMinimalApiResult());
        employeeGroup.MapDelete("/{id}", async ([FromRoute] string id, ISender sender)
            => (await sender.Send(new DeleteEmployeeCommand(id))).ToMinimalApiResult());
        employeeGroup.MapPost("/", async ([FromBody] CreateEmployeeCommand command, ISender sender)
            => (await sender.Send(command)).ToMinimalApiResult());
        employeeGroup.MapPut("/{id}", async ([FromRoute]string id, [FromBody] UpdateEmployeeCommand command, ISender sender)
            => (await sender.Send(command with { Id = id})).ToMinimalApiResult());
    }
}