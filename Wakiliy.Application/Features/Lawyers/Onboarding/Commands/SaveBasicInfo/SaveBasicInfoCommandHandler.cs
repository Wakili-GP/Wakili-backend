using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;

public class SaveBasicInfoCommandHandler(
    UserManager<AppUser> userManager,
    ILawyerRepository lawyerRepository,
    ISpecializationRepository specializationRepository)
    : IRequestHandler<SaveBasicInfoCommand, Result<OnboardingStepResponse<BasicInfoDataDto>>>
{
    public async Task<Result<OnboardingStepResponse<BasicInfoDataDto>>> Handle(SaveBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (lawyer is null)
            return Result.Failure<OnboardingStepResponse<BasicInfoDataDto>>(OnboardingErrors.LawyerNotFound);

        var selectedIds = request.PracticeAreas.Distinct().ToList();
        var selectedSpecializations = await specializationRepository.GetByIdsAsync(selectedIds, cancellationToken);

        if (selectedSpecializations.Count != selectedIds.Count || selectedSpecializations.Any(s => !s.IsActive))
            return Result.Failure<OnboardingStepResponse<BasicInfoDataDto>>(SpecializationErrors.InvalidSelection);

        request.Adapt(lawyer);

        lawyer.Specializations.Clear();
        foreach (var specialization in selectedSpecializations)
            lawyer.Specializations.Add(specialization);

        if (!string.IsNullOrEmpty(request.ProfileImage))
            lawyer.ImageUrl = request.ProfileImage;

        lawyer.MarkStepCompleted(LawyerOnboardingSteps.BasicInfo, LawyerOnboardingSteps.Education);

        var result = await userManager.UpdateAsync(lawyer);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<OnboardingStepResponse<BasicInfoDataDto>>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var responseData = lawyer.Adapt<BasicInfoDataDto>();
        responseData.PracticeAreas = lawyer.Specializations.Adapt<List<SpecializationOptionDto>>();
        responseData.ProfileImage = lawyer.ImageUrl;


        var response = LawyerOnboardingHelper.BuildResponse(lawyer, responseData, "Basic info saved successfully");
        return Result.Success(response);
    }
  
}
