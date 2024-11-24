using System.Text;
using Application.Auth;
using Application.Auth.Jwt;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class InfrastructureConf
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddAuth(services);
        services.AddScoped<IUser, CurrentUser>();
        return services;
    }

    public static void AddAuth(IServiceCollection services)
    {
        services.AddDbContext<AuthDbContext>(
            options => options.UseSqlite("Data Source=app.db"));
        services.AddAuthorization();
        services.AddIdentity<ApplicationUser, IdentityRole>(opt => {
            opt.Password.RequiredLength = 5;
            opt.User.RequireUniqueEmail = true;
            opt.SignIn.RequireConfirmedEmail = false;
        }).AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

        services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt => {
            var serviceProvider = services.BuildServiceProvider();
            var jwtOptions = serviceProvider.GetService<IOptions<JWTOptions>>()!.Value;

            opt.SaveToken = true;
            opt.RequireHttpsMetadata = false;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = jwtOptions.ValidAudience,
                ValidIssuer = jwtOptions.ValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            };
        });
    }
}