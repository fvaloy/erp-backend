using Application.Auth;
using Application.Auth.Jwt;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
public sealed record LoginResponse(string AccessToken, string RefreshToken);

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager, 
    SignInManager<ApplicationUser> signInManager,
    TokenManager tokenManager) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signManager = signInManager;
    private readonly TokenManager _tokenManager = tokenManager;
    
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Error("There is no user with that email");
        }

        var signInResult = await _signManager.PasswordSignInAsync(user, request.Password, false, false);

        if (!signInResult.Succeeded) return Result.Error("Incorrect password");

        var accessToken = await _tokenManager.CreateToken(user);
        var refreshToken = _tokenManager.CreateRefreshToken();
        user.AddRefreshToken(refreshToken);

        await _userManager.UpdateAsync(user);

        return Result<LoginResponse>.Success(new(accessToken, refreshToken));
    }
}


public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} is required");
    }
}