using Api;
using Application;
using Application.UseCases.HelloWorld.Queries;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureApi()
        .ConfigureApplication();
}

var app = builder.Build();
{
    app.MapGet("/", async (ISender sender) => await sender.Send(new HelloWorldQuery()));

    app.Run();
}