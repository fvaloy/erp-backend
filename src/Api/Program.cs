using Api;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .AddApi()
        .AddInfrastructure()
        .AddApplication();
}

var app = builder.Build();
{
    app.ConfigureMiddlewareException();
    app.UseCors(ApiConf.CORS_POLICY);

    app.MapEndpoints();

    app.Run();
}