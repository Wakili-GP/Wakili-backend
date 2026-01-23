using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.Update
{
    public class UpdateAccountCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<UpdateAccountCommand, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure<UserInfoResponse>(new Error("User.NotFound", "User not found", StatusCodes.Status404NotFound));
            }

            request.Adapt(user);

            var update = await userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                var err = update.Errors.First();
                return Result.Failure<UserInfoResponse>(new Error(err.Code, err.Description, StatusCodes.Status400BadRequest));
            }

            return Result.Success(user.Adapt<UserInfoResponse>());
        }
    }
}