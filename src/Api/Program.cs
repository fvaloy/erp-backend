using Api;
using Application;
using Application.UseCases.Greeting.Queries;
using Francisvac.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureApi()
        .ConfigureApplication();
}

var app = builder.Build();
{
    app.ConfigureMiddlewareException();

    app.MapGet("/greeting", async ([FromQuery] string name, ISender sender) => (await sender.Send(new GreetingQuery(name))).ToMinimalApiResult());

    app.Run();
}