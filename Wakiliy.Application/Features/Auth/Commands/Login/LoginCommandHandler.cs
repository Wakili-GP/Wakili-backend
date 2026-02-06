using Mapster;
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
    IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            // generate token
            (string token, int expiresIn) = jwtProvider.GenerateToken(user, userRoles);


            var userDto = user.Adapt<UserDto>();
            userDto.UserType = userRoles.FirstOrDefault() ?? "Client";

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
