using Api;
using Application;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureApi()
        .ConfigureApplication();
}

var app = builder.Build();
{
    app.ConfigureMiddlewareException();
    app.UseCors(ApiConf.CORS_POLICY);

    app.MapEndpoints();

    app.Run();
}