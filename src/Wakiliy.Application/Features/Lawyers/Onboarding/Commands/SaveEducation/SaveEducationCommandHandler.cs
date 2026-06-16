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
    IUnitOfWork unitOfWork,
    IFileUploadService fileUploadService)
    : IRequestHandler<SaveEducationCommand, Result<OnboardingStepResponse<EducationDataDto>>>
{
    public async Task<Result<OnboardingStepResponse<EducationDataDto>>> Handle(SaveEducationCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await unitOfWork.Lawyers.GetByIdWithQualificationsAndCertificationsAsync(request.UserId);

        if (lawyer is null)
            return Result.Failure<OnboardingStepResponse<EducationDataDto>>(OnboardingErrors.LawyerNotFound);

        if (!lawyer.CanAccessStep(LawyerOnboardingSteps.Education))
            return Result.Failure<OnboardingStepResponse<EducationDataDto>>(OnboardingErrors.StepPrerequisite(LawyerOnboardingSteps.BasicInfo));

        await unitOfWork.AcademicQualifications.DeleteByLawyerIdAsync(request.UserId, cancellationToken);

        foreach (var q in request.AcademicQualifications)
        {
            var qualification = new AcademicQualification
            {
                LawyerId = request.UserId,
                DegreeType = q.DegreeType,
                FieldOfStudy = q.FieldOfStudy,
                UniversityName = q.UniversityName,
                GraduationYear = ParseYear(q.GraduationYear),
            };

            if (q.Document != null)
            {
                qualification.Document = await SaveAndUploadFileAsync(q.Document, request.UserId, FilePurpose.Verification, FileCategory.EducationalCertificate, cancellationToken);
            }

            lawyer.AcademicQualifications.Add(qualification);
        }


        if(request.ProfessionalCertifications != null)
        {

            await unitOfWork.ProfessionalCertifications.DeleteByLawyerIdAsync(request.UserId, cancellationToken);

            foreach (var certification in request.ProfessionalCertifications)
            {
                var document = certification.Document != null
                    ? await SaveAndUploadFileAsync(certification.Document, request.UserId, FilePurpose.Verification, FileCategory.ProfessionalCertificate, cancellationToken)
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

        await unitOfWork.Lawyers.UpdateAsync(lawyer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var professionalCertificationsResposne = lawyer.ProfessionalCertifications.Select(c => new ProfessionalCertificationResponseDto
        {
            CertificateName = c.CertificateName,
            IssuingOrganization = c.IssuingOrganization,
            YearObtained = c.YearObtained.ToString(),
            Document = c.Document != null ? c.Document.SystemFileUrl : null
        }).ToList();

        var acacdemicQualificationsResponce = lawyer.AcademicQualifications.Select(q => new AcademicQualificationResponseDto
        {
            DegreeType = q.DegreeType,
            FieldOfStudy = q.FieldOfStudy,
            UniversityName = q.UniversityName,
            GraduationYear = q.GraduationYear.ToString(),
            Document = q.Document?.SystemFileUrl
        }).ToList();



        var response = LawyerOnboardingHelper.BuildResponse(lawyer, new EducationDataDto
        {
            AcademicQualifications = acacdemicQualificationsResponce,
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

    private async Task<UploadedFile> SaveAndUploadFileAsync(IFormFile profileImage, string userId, FilePurpose purpose, FileCategory category, CancellationToken cancellationToken)
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

            Category = category,
            Purpose = purpose,
        };

        fileEntity.SystemFileUrl = $"/api/files/{fileEntity.Id}";

        await unitOfWork.UploadedFiles.AddAsync(fileEntity, cancellationToken);

        return fileEntity;
    }
}
