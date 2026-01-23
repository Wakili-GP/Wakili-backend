using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.ToggleStatus
{
    public class ToggleLawyerActiveStatusCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ToggleLawyerActiveStatusCommand, Result>
    {
        public async Task<Result> Handle(ToggleLawyerActiveStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is not Lawyer lawyer)
                return Result.Failure(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));

            lawyer.IsActive = !lawyer.IsActive;

            await userManager.UpdateAsync(lawyer);

            return Result.Success();
        }
    }
}