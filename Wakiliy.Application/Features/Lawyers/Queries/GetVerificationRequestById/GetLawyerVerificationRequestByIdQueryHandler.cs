using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequestById
{
    public class GetLawyerVerificationRequestByIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetLawyerVerificationRequestByIdQuery, Result<LawyerVerificationDetailResponse>>
    {
        public async Task<Result<LawyerVerificationDetailResponse>> Handle(
            GetLawyerVerificationRequestByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Get lawyer with all related data
            var lawyer = await unitOfWork.Lawyers.GetByIdWithAllOnboardingDataAsync(request.Id);

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
                SessionTypes = lawyer.SessionTypes.Select(st => Enum.TryParse<SessionType>(st, out var sessionType) ? (int)sessionType : 0).ToList(),
                Education = lawyer.AcademicQualifications.Select(aq => new EducationDto
                {
                    DegreeType = aq.DegreeType,
                    FieldOfStudy = aq.FieldOfStudy,
                    University = aq.UniversityName,
                    GraduationYear = aq.GraduationYear.ToString(),
                    Document = aq.Document?.SystemFileUrl
                }).ToList(),
                Certifications = lawyer.ProfessionalCertifications.Select(pc => new CertificationDto
                {
                    Name = pc.CertificateName,
                    IssuingOrg = pc.IssuingOrganization,
                    YearObtained = pc.YearObtained.ToString(),
                    Document = pc.Document?.SystemFileUrl
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
                Verification = MapVerification(lawyer),
            };

            return Result.Success(response);
        }

        private VerificationDto MapVerification(Lawyer lawyer)
        {
            var docs = new VerificationDto
            {
                LawyerLicenseNumber = lawyer.LicenseNumber,
                LawyerLicenseIssuingAuthority = lawyer.IssuingAuthority,
                LawyerLicenseYearOfIssue = lawyer.LicenseYear
            };

            if (lawyer.VerificationDocuments == null || !lawyer.VerificationDocuments.Any())
                return docs;
            
            var nationalIdFront = lawyer.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdFront);
            var nationalIdBack = lawyer.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdBack);
            var lawyerLicense = lawyer.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.LawyerLicense);

            docs.NationalIdFront = nationalIdFront?.File?.SystemFileUrl;
            docs.NationalIdBack = nationalIdBack?.File?.SystemFileUrl;
            docs.LawyerLicense = lawyerLicense?.File?.SystemFileUrl;

            return docs;
        }

        
    }
}
