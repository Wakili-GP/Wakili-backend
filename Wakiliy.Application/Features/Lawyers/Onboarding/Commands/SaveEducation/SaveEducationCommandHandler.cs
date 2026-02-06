using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveEducation;

public class SaveEducationCommandHandler(ILawyerRepository lawyerRepository)
    : IRequestHandler<SaveEducationCommand, Result<OnboardingStepResponse<EducationDataDto>>>
{
    public async Task<Result<OnboardingStepResponse<EducationDataDto>>> Handle(SaveEducationCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdWithQualificationsAndCertificationsAsync(request.UserId);

        if (lawyer is null)
            return Result.Failure<OnboardingStepResponse<EducationDataDto>>(OnboardingErrors.LawyerNotFound);

        if (!lawyer.CanAccessStep(LawyerOnboardingSteps.Education))
            return Result.Failure<OnboardingStepResponse<EducationDataDto>>(OnboardingErrors.StepPrerequisite(LawyerOnboardingSteps.BasicInfo));

        lawyer.AcademicQualifications.Clear();
        foreach (var qualification in request.AcademicQualifications)
        {
            lawyer.AcademicQualifications.Add(new AcademicQualification
            {
                DegreeType = qualification.DegreeType,
                FieldOfStudy = qualification.FieldOfStudy,
                UniversityName = qualification.UniversityName,
                GraduationYear = ParseYear(qualification.GraduationYear)
            });
        }

        if(request.ProfessionalCertifications != null)
        {
            lawyer.ProfessionalCertifications.Clear();
            foreach (var certification in request.ProfessionalCertifications)
            {
                lawyer.ProfessionalCertifications.Add(new ProfessionalCertification
                {
                    CertificateName = certification.CertificateName,
                    IssuingOrganization = certification.IssuingOrganization,
                    YearObtained = ParseYear(certification.YearObtained),
                    DocumentPath = certification.Document
                });
            }
        }
        

        lawyer.MarkStepCompleted(LawyerOnboardingSteps.Education, LawyerOnboardingSteps.Experience);

        await lawyerRepository.UpdateAsync(lawyer);

        var response = LawyerOnboardingHelper.BuildResponse(lawyer, new EducationDataDto
        {
            AcademicQualifications = request.AcademicQualifications,
            ProfessionalCertifications = request.ProfessionalCertifications
        }, "Education info saved");

        return Result.Success(response);
    }

    private static int ParseYear(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var year)
            ? year
            : 0;
    }
}
