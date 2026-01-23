using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Update
{
    public class UpdateLawyerCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<UpdateLawyerCommand, Result<LawyerResponse>>
    {
        public async Task<Result<LawyerResponse>> Handle(UpdateLawyerCommand request, CancellationToken cancellationToken)
        {
            var lawyer = await userManager.Users
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

            if (lawyer is null)
            {
                return Result.Failure<LawyerResponse>(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));
            }

            request.Adapt(lawyer);

            var result = await userManager.UpdateAsync(lawyer);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<LawyerResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            return Result.Success<LawyerResponse>(lawyer.Adapt<LawyerResponse>());
        }
    }
}
