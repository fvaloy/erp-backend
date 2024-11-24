using System.Reflection;
using Application.Auth.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth;

public class AuthorizationBehaviour<TRequest, TResponse>(IUser user, UserManager<ApplicationUser> userManager) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUser _user = user;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            if (_user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var user = await _userManager.FindByIdAsync(_user.Id);
                        if (user is null)
                        {
                            authorized = false;
                            break;
                        }

                        var isInRole = await _userManager.IsInRoleAsync(user!, role);
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }
        }
        return await next();
    }
}