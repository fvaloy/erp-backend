using Api;

var builder = WebApplication.CreateBuilder(args);
{
    builder.ConfigureApi();
}

var app = builder.Build();
{
    app.MapGet("/", (ILogger<Program> logger) => {
        logger.LogInformation("Hello World!");
        return Results.Ok("Hello World!");
    });

    app.Run();
}