using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Auth.Jwt;

public class JWTOptions
{
    public string Secret { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
}

public class JWTOptionsSetup(IConfiguration configuration) : IConfigureOptions<JWTOptions>
{
    private readonly IConfiguration _configuration = configuration;
    private const string ConfigurationSectionName = "jwt";

    public void Configure(JWTOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}