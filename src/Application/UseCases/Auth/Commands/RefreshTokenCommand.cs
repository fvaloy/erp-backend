using System.Security.Claims;
using Application.Auth;
using Application.Auth.Jwt;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Auth.Commands;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;
public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken);

public sealed class RefreshTokenCommandHandler(
    TokenManager tokenManager,
    UserManager<ApplicationUser> userManager
) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly TokenManager _tokenManager = tokenManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userPrincipal = _tokenManager.GetPrincipalsFromExpireToken(request.AccessToken);
        var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await _userManager.FindByIdAsync(userId);

        if (user!.RefreshTokenExpireTime < DateTime.Now)
        {
            return Result.Error("RefreshToken expired");
        }

        var newAccessToken = await _tokenManager.CreateToken(user);
        var newRefreshToken = _tokenManager.CreateRefreshToken();
        user.AddRefreshToken(newRefreshToken);
        await _userManager.UpdateAsync(user);

        return Result<RefreshTokenResponse>.Success(new(newAccessToken, newRefreshToken));
    }
}

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("{PropertyName} is required");
    }
}