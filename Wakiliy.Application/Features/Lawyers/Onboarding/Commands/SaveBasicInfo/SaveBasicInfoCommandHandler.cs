using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;

public class SaveBasicInfoCommandHandler(
    ILawyerRepository lawyerRepository,
    ISpecializationRepository specializationRepository,
    IFileUploadService fileUploadService,
    IUploadedFileRepository uploadedFileRepository)
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

        BasicInfoDataDto responseData = new();


        if (request.ProfileImage is not null)
        {
            lawyer.ProfileImage = await SaveAndUploadImageAsync(request.ProfileImage,lawyer.Id,cancellationToken);
        }

        lawyer.MarkStepCompleted(LawyerOnboardingSteps.BasicInfo, LawyerOnboardingSteps.Education);

        await lawyerRepository.UpdateAsync(lawyer,cancellationToken);

        responseData = lawyer.Adapt<BasicInfoDataDto>();
        responseData.PracticeAreas = lawyer.Specializations.Adapt<List<SpecializationOptionDto>>();
        responseData.ProfileImage = lawyer.ProfileImage?.SystemFileUrl;


        var response = LawyerOnboardingHelper.BuildResponse(lawyer, responseData, "Basic info saved successfully");
        return Result.Success(response);
    }

    private async Task<UploadedFile> SaveAndUploadImageAsync(IFormFile profileImage,string userId,CancellationToken cancellationToken)
    {
        var uploadResult = await fileUploadService.UploadAsync(profileImage, "uploads");

        var fileEntity = new UploadedFile
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,

            FileName = uploadResult.FileName,
            PublicId = uploadResult.PublicId,
            Size = uploadResult.Size,
            ContentType = uploadResult.ContentType,

            Category = FileCategory.ProfilePicture,
            Purpose = FilePurpose.Profile,
        };

        fileEntity.SystemFileUrl = $"/api/files/{fileEntity.Id}";

        await uploadedFileRepository.AddAsync(fileEntity,cancellationToken);

        return fileEntity;
    }
}
