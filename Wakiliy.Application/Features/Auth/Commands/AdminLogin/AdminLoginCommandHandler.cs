using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.AdminLogin;

public class AdminLoginCommandHandler(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtProvider jwtProvider,ILogger<AdminLoginCommandHandler> logger) : IRequestHandler<AdminLoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        var isAdmin = await userManager.IsInRoleAsync(user, DefaultRoles.Admin);
        var isSuperAdmin = await userManager.IsInRoleAsync(user, DefaultRoles.SuperAdmin);
        if (!isAdmin && !isSuperAdmin)
        {
            logger.LogWarning("Unauthorized login attempt for email: {Email}", request.Email);
            return Result.Failure<LoginResponse>(UserErrors.Unauthorized);
        }

        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            // Generate token
            (string token, int expiresIn) = jwtProvider.GenerateToken(user, userRoles);

            var userDto = user.Adapt<UserDto>();
            userDto.UserType = isAdmin ? DefaultRoles.Admin : DefaultRoles.SuperAdmin;

            var loginResponse = new LoginResponse
            {
                AccessToken = token,
                RefreshToken = "refreshToken",
                ExpiresIn = expiresIn,
                User = userDto
            };

            return Result.Success(loginResponse);
        }

        if (result.IsLockedOut)
            return Result.Failure<LoginResponse>(UserErrors.LockedUser);

        return Result.Failure<LoginResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }
}
