using System.Globalization;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;

public class SaveExperienceCommandHandler(ILawyerRepository lawyerRepository)
    : IRequestHandler<SaveExperienceCommand, Result<OnboardingStepResponse<ExperienceDataDto>>>
{
    public async Task<Result<OnboardingStepResponse<ExperienceDataDto>>> Handle(SaveExperienceCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdWithExperiencesAsync(request.UserId, cancellationToken);

        if (lawyer is null)
            return Result.Failure<OnboardingStepResponse<ExperienceDataDto>>(OnboardingErrors.LawyerNotFound);

        if (!lawyer.CanAccessStep(LawyerOnboardingSteps.Experience))
            return Result.Failure<OnboardingStepResponse<ExperienceDataDto>>(OnboardingErrors.StepPrerequisite(LawyerOnboardingSteps.Education));

        lawyer.WorkExperiences.Clear();
        foreach (var experience in request.WorkExperiences)
        {
            lawyer.WorkExperiences.Add(new WorkExperience
            {
                JobTitle = experience.JobTitle,
                OrganizationName = experience.OrganizationName,
                StartYear = ParseYear(experience.StartYear),
                EndYear = string.IsNullOrWhiteSpace(experience.EndYear) ? null : ParseYear(experience.EndYear),
                IsCurrentJob = experience.IsCurrentJob,
                Description = experience.Description
            });
        }

        lawyer.MarkStepCompleted(LawyerOnboardingSteps.Experience, LawyerOnboardingSteps.Verification);

        await lawyerRepository.UpdateAsync(lawyer,cancellationToken);

        var response = LawyerOnboardingHelper.BuildResponse(lawyer, new ExperienceDataDto
        {
            WorkExperiences = request.WorkExperiences
        }, "Experience saved");

        return Result.Success(response);
    }

    private static int ParseYear(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var year)
            ? year
            : 0;
    }
}
