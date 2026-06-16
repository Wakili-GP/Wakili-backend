using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.ChangePassword;

public class ChangePasswordCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return Result.Failure<string>(new Error("User.NotFound", "User not found", StatusCodes.Status404NotFound));
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Password change failed";
            return Result.Failure<string>(new Error("User.ChangePasswordFailed", errors, StatusCodes.Status400BadRequest));
        }

        return Result.Success("Password changed successfully");
    }
}
