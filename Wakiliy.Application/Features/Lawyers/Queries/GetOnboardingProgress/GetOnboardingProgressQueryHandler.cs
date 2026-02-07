using Mapster;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Features.Lawyers.Queries.GetOnboardingProgress;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Queries.GetOnboardingProgress
{
    public class GetOnboardingProgressQueryHandler(ILawyerRepository lawyerRepository)
        : IRequestHandler<GetOnboardingProgressQuery, Result<OnboardingProgressResponseDto>>
    {
        public async Task<Result<OnboardingProgressResponseDto>> Handle(GetOnboardingProgressQuery request, CancellationToken cancellationToken)
        {
            var lawyer = await lawyerRepository.GetByIdWithAllOnboardingDataAsync(request.UserId);

            if (lawyer is null)
                return Result.Failure<OnboardingProgressResponseDto>(OnboardingErrors.LawyerNotFound);

            var response = new OnboardingProgressResponseDto
            {
                CurrentStep = lawyer.CurrentOnboardingStep,
                CompletedSteps = lawyer.CompletedOnboardingSteps,
                LastUpdated = lawyer.UpdatedAt ?? lawyer.CreatedAt,
                Data = new OnboardingAllDataDto
                {
                    BasicInfo = MapBasicInfo(lawyer),
                    Education = MapEducation(lawyer),
                    Experience = MapExperience(lawyer),
                    Verification = MapVerification(lawyer)
                }
            };

            return Result.Success(response);
        }

        private static BasicInfoDataDto? MapBasicInfo(Lawyer lawyer)
        {
            if (!lawyer.CompletedOnboardingSteps.Contains(1)) return null;
            return lawyer.Adapt<BasicInfoDataDto>();
        }

        private static EducationDataDto? MapEducation(Lawyer lawyer)
        {
            if (!lawyer.CompletedOnboardingSteps.Contains(2)) return null;
            return new EducationDataDto
            {
                AcademicQualifications = lawyer.AcademicQualifications.Adapt<List<AcademicQualificationDto>>(),
                ProfessionalCertifications = lawyer.ProfessionalCertifications.Adapt<List<ProfessionalCertificationResponseDto>>()
            };
        }

        private static ExperienceDataDto? MapExperience(Lawyer lawyer)
        {
            if (!lawyer.CompletedOnboardingSteps.Contains(3)) return null;
            return new ExperienceDataDto
            {
                WorkExperiences = lawyer.WorkExperiences.Adapt<List<WorkExperienceDto>>()
            };
        }

        private static VerificationDocumentsDto? MapVerification(Lawyer lawyer)
        {
            if (!lawyer.CompletedOnboardingSteps.Contains(4)) return null;
            return lawyer.Adapt<VerificationDocumentsDto>();
        }
    }
}
