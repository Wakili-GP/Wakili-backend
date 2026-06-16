using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler(UserManager<AppUser> userManager)
        : IRequestHandler<UpdateUserCommand, Result<UserListItemDto>>
    {
        public async Task<Result<UserListItemDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is null)
                return Result.Failure<UserListItemDto>(UserErrors.UserNotFound);


            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure<UserListItemDto>(new Error("User.UpdateFailed", error, 400));
            }

            var roles = await userManager.GetRolesAsync(user);

            var userType = roles.Contains(DefaultRoles.Lawyer) ? DefaultRoles.Lawyer : DefaultRoles.Client;


            return Result.Success(user.Adapt<UserListItemDto>());
        }
    }
}
