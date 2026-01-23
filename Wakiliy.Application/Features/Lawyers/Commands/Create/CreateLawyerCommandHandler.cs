using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Create
{
    public class CreateLawyerCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<CreateLaywerCommand,Result<LawyerResponse>>
    {
        public async Task<Result<LawyerResponse>> Handle(CreateLaywerCommand request, CancellationToken cancellationToken)
        {
            var lawyer = new Lawyer
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                FullName = request.FullName,
                Address = request.Address,
                LicenseNumber = request.LicenseNumber,
                Specialization = request.Specialization,
                YearsOfExperience = request.YearsOfExperience,
                VerificationStatus = request.VerificationStatus,
                JoinedDate = DateTime.UtcNow,
                IsActive = true
            };

            var result = await userManager.CreateAsync(lawyer, "Temp@123");
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<LawyerResponse>(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
            }

            await userManager.AddToRoleAsync(lawyer, DefaultRoles.Lawyer);

            return Result.Success(lawyer.Adapt<LawyerResponse>());
        }
    }
}
