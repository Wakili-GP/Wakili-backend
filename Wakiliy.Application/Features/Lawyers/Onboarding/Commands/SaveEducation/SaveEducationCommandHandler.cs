using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveEducation;

public class SaveEducationCommandHandler(
    ILawyerRepository lawyerRepository,
    IProfessionalCertificationRepository professionalCertificationRepository,
    IAcademicQualificationRepository academicQualificationRepository,
    IUploadedFileRepository uploadedFileRepository,
    IFileUploadService fileUploadService)
    : IRequestHandler<SaveEducationCommand, Result<OnboardingStepResponse<EducationDataDto>>>
{
    public async Task<Result<OnboardingStepResponse<EducationDataDto>>> Handle(SaveEducationCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdWithQualificationsAndCertificationsAsync(request.UserId);

        if (lawyer is null)
            return Result.Failure<OnboardingStepResponse<EducationDataDto>>(OnboardingErrors.LawyerNotFound);

        if (!lawyer.CanAccessStep(LawyerOnboardingSteps.Education))
            return Result.Failure<OnboardingStepResponse<EducationDataDto>>(OnboardingErrors.StepPrerequisite(LawyerOnboardingSteps.BasicInfo));

        //lawyer.AcademicQualifications.Clear();
        await academicQualificationRepository.DeleteByLawyerIdAsync(request.UserId,cancellationToken);

        lawyer.AcademicQualifications = request.AcademicQualifications
            .Select(q => new AcademicQualification
            {
                LawyerId = request.UserId,
                DegreeType = q.DegreeType,
                FieldOfStudy = q.FieldOfStudy,
                UniversityName = q.UniversityName,
                GraduationYear = ParseYear(q.GraduationYear)
            })
            .ToList();


        if(request.ProfessionalCertifications != null)
        {

            await professionalCertificationRepository.DeleteByLawyerIdAsync(request.UserId, cancellationToken);

            foreach (var certification in request.ProfessionalCertifications)
            {
                var document = certification.Document != null
                    ? await SaveAndUploadFileAsync(certification.Document, request.UserId, cancellationToken)
                    : null;


                lawyer.ProfessionalCertifications.Add(new ProfessionalCertification
                {
                    LawyerId = request.UserId,
                    CertificateName = certification.CertificateName,
                    IssuingOrganization = certification.IssuingOrganization,
                    YearObtained = ParseYear(certification.YearObtained),
                    Document = document
                });
            }

        }
        

        lawyer.MarkStepCompleted(LawyerOnboardingSteps.Education, LawyerOnboardingSteps.Experience);

        await lawyerRepository.UpdateAsync(lawyer);

        var professionalCertificationsResposne = lawyer.ProfessionalCertifications.Select(c => new ProfessionalCertificationResponseDto
        {
            CertificateName = c.CertificateName,
            IssuingOrganization = c.IssuingOrganization,
            YearObtained = c.YearObtained.ToString(),
            DocumentPath = c.Document != null ? c.Document.SystemFileUrl : null
        }).ToList();



        var response = LawyerOnboardingHelper.BuildResponse(lawyer, new EducationDataDto
        {
            AcademicQualifications = request.AcademicQualifications,
            ProfessionalCertifications = professionalCertificationsResposne
        }, "Education info saved");

        return Result.Success(response);
    }

    private static int ParseYear(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var year)
            ? year
            : 0;
    }

    private async Task<UploadedFile> SaveAndUploadFileAsync(IFormFile profileImage, string userId, CancellationToken cancellationToken)
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

        await uploadedFileRepository.AddAsync(fileEntity, cancellationToken);

        return fileEntity;
    }
}
