using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.Login;
public class LoginCommandHandler(UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Include(x=>x.ProfileImage).Where(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        if(user.Status == UserStatus.Inactive)
            return Result.Failure<LoginResponse>(UserErrors.InactiveUser);

        // Admins must use the admin-login endpoint
        var userRoles = await userManager.GetRolesAsync(user);
        if (userRoles.Contains(DefaultRoles.Admin))
            return Result.Failure<LoginResponse>(UserErrors.Unauthorized);

        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // generate token
            (string token, int expiresIn) = jwtProvider.GenerateToken(user, userRoles);

            var userDto = user.Adapt<UserDto>();
            userDto.UserType = userRoles.FirstOrDefault() ?? DefaultRoles.Client;
            
            if (user is Lawyer lawyer)
            {
                if (lawyer.CurrentOnboardingStep < LawyerOnboardingSteps.Completed)
                    userDto.Status = LawyerOnboardingStatus.Unfinished.ToString();
                else if (lawyer.VerificationStatus != VerificationStatus.Approved)
                    userDto.Status = LawyerOnboardingStatus.SubmittedAndNotApproved.ToString();
                else
                    userDto.Status = LawyerOnboardingStatus.SubmittedAndApproved.ToString();
            }

            userDto.profileImage = user.ProfileImage?.SystemFileUrl;


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
