using MediatR;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.DeleteAdmin
{
    public class DeleteAdminCommandHandler(
        UserManager<AppUser> userManager) 
        : IRequestHandler<DeleteAdminCommand, Result>
    {
        public async Task<Result> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            // Find user by ID
            var user = await userManager.FindByIdAsync(request.Id);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFound);

            // Verify user is an admin
            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Contains(DefaultRoles.Admin))
                return Result.Failure(new Error("Admin.NotFound", "User is not an admin", 404));

            // Delete the admin user
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(new Error("Admin.DeleteFailed", error, 400));
            }

            return Result.Success();
        }
    }
}
