using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Delete
{
    public class DeleteLawyerCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<DeleteLawyerCommand, Result>
    {
        public async Task<Result> Handle(DeleteLawyerCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is not Lawyer lawyer)
                return Result.Failure(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));

            var result = await userManager.DeleteAsync(lawyer);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }
    }
}