using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.UpdateAdmin
{
    public class UpdateAdminCommandHandler(
        UserManager<AppUser> userManager) 
        : IRequestHandler<UpdateAdminCommand, Result<AdminDto>>
    {
        public async Task<Result<AdminDto>> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
        {
            // Find user by ID
            var user = await userManager.FindByIdAsync(request.Id);
            if (user == null)
                return Result.Failure<AdminDto>(UserErrors.UserNotFound);

            // Verify user is an admin
            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Contains(DefaultRoles.Admin))
                return Result.Failure<AdminDto>(new Error("Admin.NotFound", "User is not an admin", 404));

            // Update status if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<UserStatus>(request.Status, true, out var status))
                {
                    user.Status = status;
                    user.UpdatedAt = DateTime.UtcNow;
                }
            }

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure<AdminDto>(new Error("Admin.UpdateFailed", error, 400));
            }

            // Map to DTO
            var adminDto = new AdminDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = DefaultRoles.Admin,
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt
            };

            return Result.Success(adminDto);
        }
    }
}
