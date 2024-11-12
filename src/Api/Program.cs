using Api;
using Application;
using Application.UseCases.Greeting.Queries;
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
    app.MapGet("/greeting", async ([FromQuery] string name, ISender sender) => await sender.Send(new GreetingQuery(name)));

    app.Run();
}