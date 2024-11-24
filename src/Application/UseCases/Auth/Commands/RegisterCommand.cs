using Application.Auth;
using Application.Auth.Jwt;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth.Commands;

public sealed record RegisterCommand(
    string Email,
    string UserName,
    string Password,
    string? PhoneNumber
) : IRequest<Result<string>>;

public sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    TokenManager tokenGenerator,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly TokenManager _tokenGenerator = tokenGenerator;
    private readonly ILogger<RegisterCommandHandler> _logger = logger;

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExist = await _userManager.FindByEmailAsync(request.Email);
        if (emailExist is not null) return Result<string>.Error("There is already a user with that email");

        ApplicationUser user = new()
        {
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
        };

        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) return Result<string>.Error(string.Join("\n", result.Errors.Select(e => $"Code: {e.Code} | Description: {e.Description}")));
        }

        {
            var result = await _userManager.AddToRoleAsync(user, UserRoles.USER);
            _logger.LogWarning($"Could not assign {UserRoles.USER} role. \n Reason: \n {string.Join("\n", result.Errors.Select(e => $"Code: {e.Code} | Description: {e.Description}"))}");
        }

        return Result<string>.Success("The user was registered correctly");
    }
}

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} is required");
    }
}