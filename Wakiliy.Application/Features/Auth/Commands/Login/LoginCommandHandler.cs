using MediatR;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.Login;
public class LoginCommandHandler(UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            // generate token
            (string token, int expiresIn) = jwtProvider.GenerateToken(user, userRoles);

            return Result.Success(new AuthResponse(user.Id, user.Email!, user.FullName, token, expiresIn));
        }

        if (result.IsLockedOut)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }
}
