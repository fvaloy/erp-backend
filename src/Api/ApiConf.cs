using Serilog;

namespace Api;

public static class ApiConf
{
    public static IServiceCollection ConfigureApi(this WebApplicationBuilder builder)
    {
        ConfigureLoggin(builder);
        return builder.Services;
    }

    public static void ConfigureLoggin(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
    }
}