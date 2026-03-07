using MediatR;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.ToggleUserStatus
{
    public class ToggleUserStatusCommandHandler(UserManager<AppUser> userManager)
        : IRequestHandler<ToggleUserStatusCommand, Result>
    {
        public async Task<Result> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            user.Status = user.Status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(new Error("User.ToggleStatusFailed", error, 400));
            }

            return Result.Success();
        }
    }
}
