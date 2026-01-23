using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetInfo
{
    public class GetAccountInfoQueryHandler(
        UserManager<AppUser> userManager) : IRequestHandler<GetAccountInfoQuery, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(GetAccountInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);

            if (user is null)
                return Result.Failure<UserInfoResponse>(new Error("User.NotFound", "User not found",StatusCodes.Status404NotFound));

            return Result.Success(user.Adapt<UserInfoResponse>());
        }
    }
}