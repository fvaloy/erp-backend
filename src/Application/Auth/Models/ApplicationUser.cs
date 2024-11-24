using Microsoft.AspNetCore.Identity;

namespace Application.Auth;

public class ApplicationUser : IdentityUser
{
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpireTime { get; set; }

    public void AddRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpireTime = DateTime.Now.AddHours(5);
    }
}