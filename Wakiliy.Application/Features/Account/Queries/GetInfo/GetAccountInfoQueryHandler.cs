using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetInfo
{
    public class GetAccountInfoQueryHandler(
        UserManager<AppUser> userManager) : IRequestHandler<GetAccountInfoQuery, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(GetAccountInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users.Include(u => u.ProfileImage).FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user is null)
                return Result.Failure<UserInfoResponse>(new Error("User.NotFound", "User not found",StatusCodes.Status404NotFound));

            var roles = await userManager.GetRolesAsync(user);
            var userType = roles.Contains(DefaultRoles.Lawyer) ? "Lawyer" : "Client";

            var response = user.Adapt<UserInfoResponse>();
            response.UserType = userType;
            response.IsEmailVerified = user.EmailConfirmed;
            response.ImageUrl = user.ProfileImage?.SystemFileUrl;

            return Result.Success(response);
        }
    }
}