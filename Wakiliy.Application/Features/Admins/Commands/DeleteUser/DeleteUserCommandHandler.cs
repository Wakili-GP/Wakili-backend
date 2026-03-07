using MediatR;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(UserManager<AppUser> userManager)
        : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            // Admins cannot be deleted through this endpoint
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains(DefaultRoles.Admin))
                return Result.Failure(UserErrors.Unauthorized);

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(new Error("User.DeleteFailed", error, 400));
            }

            return Result.Success();
        }
    }
}
