using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequestById
{
    public class GetLawyerVerificationRequestByIdQueryHandler(ILawyerRepository lawyerRepository)
        : IRequestHandler<GetLawyerVerificationRequestByIdQuery, Result<LawyerVerificationDetailResponse>>
    {
        public async Task<Result<LawyerVerificationDetailResponse>> Handle(
            GetLawyerVerificationRequestByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Get lawyer with all related data
            var lawyer = await lawyerRepository.GetByIdWithAllOnboardingDataAsync(request.Id);

            if (lawyer == null)
                return Result.Failure<LawyerVerificationDetailResponse>(
                    new Error("Lawyer.NotFound", "Lawyer verification request not found", 404));

            // Map to detailed response
            var response = new LawyerVerificationDetailResponse
            {
                Id = lawyer.Id,
                FirstName = lawyer.FirstName,
                LastName = lawyer.LastName,
                Email = lawyer.Email,
                Phone = lawyer.PhoneNumber,
                Specialty = lawyer.Specializations.Select(s => s.Name).ToList(),
                SubmittedAt = lawyer.LastOnboardingUpdate,
                Status = lawyer.VerificationStatus.ToString(),
                ProfileImage = lawyer.ProfileImage?.SystemFileUrl,
                Bio = lawyer.Bio,
                Location = new LocationDto
                {
                    Country = lawyer.Country,
                    City = lawyer.City
                },
                YearsExperience = lawyer.YearsOfExperience,
                SessionTypes = lawyer.SessionTypes,
                Education = lawyer.AcademicQualifications.Select(aq => new EducationDto
                {
                    DegreeType = aq.DegreeType,
                    FieldOfStudy = aq.FieldOfStudy,
                    University = aq.UniversityName,
                    GraduationYear = aq.GraduationYear.ToString()
                }).ToList(),
                Certifications = lawyer.ProfessionalCertifications.Select(pc => new CertificationDto
                {
                    Name = pc.CertificateName,
                    IssuingOrg = pc.IssuingOrganization,
                    YearObtained = pc.YearObtained.ToString(),
                    DocumentUrl = pc.Document?.SystemFileUrl
                }).ToList(),
                WorkExperience = lawyer.WorkExperiences.Select(we => new WorkExperienceDto
                {
                    JobTitle = we.JobTitle,
                    OrganizationName = we.OrganizationName,
                    StartYear = we.StartYear.ToString(),
                    EndYear = we.EndYear?.ToString() ?? string.Empty,
                    IsCurrentJob = we.IsCurrentJob,
                    Description = we.Description
                }).ToList(),
                Documents = MapDocuments(lawyer.VerificationDocuments),
                LicenseNumber = lawyer.LicenseNumber,
                IssuingAuthority = lawyer.IssuingAuthority,
                LicenseYear = lawyer.LicenseYear,
                BarNumber = lawyer.LicenseNumber // Assuming BarNumber is same as LicenseNumber
            };

            return Result.Success(response);
        }

        private DocumentsDto MapDocuments(ICollection<VerificationDocuments>? verificationDocuments)
        {
            if (verificationDocuments == null || !verificationDocuments.Any())
                return new DocumentsDto();

            var docs = new DocumentsDto();
            
            var nationalIdFront = verificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdFront);
            var nationalIdBack = verificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdBack);
            var lawyerLicense = verificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.LawyerLicense);
            var educationalCerts = verificationDocuments.Where(d => d.Type == VerificationDocumentType.EducationalCertificate).ToList();

            docs.GovernmentId = nationalIdFront != null && nationalIdBack != null;
            docs.GovernmentIdUrl = nationalIdFront?.File?.SystemFileUrl;
            docs.ProfessionalLicense = lawyerLicense != null;
            docs.ProfessionalLicenseUrl = lawyerLicense?.File?.SystemFileUrl;
            docs.IdentityVerification = docs.GovernmentId && docs.ProfessionalLicense;

            docs.EducationCertificates = educationalCerts.Select(ec => new EducationCertificateDto
            {
                Name = ec.File?.FileName ?? "Certificate",
                Url = ec.File?.SystemFileUrl,
                Type = GetFileExtension(ec.File?.ContentType),
                UploadedAt = ec.File?.UploadedAt ?? DateTime.UtcNow
            }).ToList();

            return docs;
        }

        private string GetFileExtension(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return "unknown";

            return contentType.ToLower() switch
            {
                "application/pdf" => "pdf",
                "image/jpeg" or "image/jpg" => "jpg",
                "image/png" => "png",
                _ => "unknown"
            };
        }
    }
}
