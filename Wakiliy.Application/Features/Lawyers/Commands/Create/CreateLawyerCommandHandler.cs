using System;
using System.Linq;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Create
{
    public class CreateLawyerCommandHandler(UserManager<AppUser> userManager, ISpecializationRepository specializationRepository)
        : IRequestHandler<CreateLaywerCommand, Result<LawyerResponse>>
    {
        public async Task<Result<LawyerResponse>> Handle(CreateLaywerCommand request, CancellationToken cancellationToken)
        {
            var specializationIds = request.SpecializationIds.Distinct().ToList();
            var specializations = await specializationRepository.GetByIdsAsync(specializationIds, cancellationToken);
            if (!specializationIds.Any() || specializations.Count != specializationIds.Count)
            {
                return Result.Failure<LawyerResponse>(SpecializationErrors.InvalidSelection);
            }

            var lawyer = new Lawyer
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                LicenseNumber = request.LicenseNumber,
                YearsOfExperience = request.YearsOfExperience,
                VerificationStatus = request.VerificationStatus,
                JoinedDate = DateTime.UtcNow,
                IsActive = true
            };

            foreach (var specialization in specializations)
            {
                lawyer.Specializations.Add(specialization);
            }

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
