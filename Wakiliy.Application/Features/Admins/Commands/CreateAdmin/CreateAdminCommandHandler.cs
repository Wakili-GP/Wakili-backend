using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager) 
        : IRequestHandler<CreateAdminCommand, Result<AdminDto>>
    {
        public async Task<Result<AdminDto>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            var emailExists = await userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExists)
                return Result.Failure<AdminDto>(UserErrors.DuplicatedEmail);

            // Validate role exists
            var roleExists = await roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)
                return Result.Failure<AdminDto>(UserErrors.InvalidRoles);

            // Create admin user
            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                NormalizedUserName = request.Email.ToUpper(),
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure<AdminDto>(new Error("User.CreateFailed", error, StatusCodes.Status400BadRequest));
            }

            // Assign admin role
            await userManager.AddToRoleAsync(user, request.Role);

            // Map to DTO
            var adminDto = new AdminDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = request.Role,
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt
            };

            return Result.Success(adminDto);
        }
    }
}
