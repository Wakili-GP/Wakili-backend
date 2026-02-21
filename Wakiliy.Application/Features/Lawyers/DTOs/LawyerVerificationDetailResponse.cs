using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

namespace Wakiliy.Application.Features.Lawyers.DTOs
{
    public class LawyerVerificationDetailResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<string> Specialty { get; set; } = new();
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? Bio { get; set; }
        public LocationDto Location { get; set; } = new();
        public int? YearsExperience { get; set; }
        public List<string> SessionTypes { get; set; } = new();
        public List<EducationDto> Education { get; set; } = new();
        public List<CertificationDto> Certifications { get; set; } = new();
        public List<WorkExperienceDto> WorkExperience { get; set; } = new();
        public DocumentsDto Documents { get; set; } = new();
        public string? LicenseNumber { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? LicenseYear { get; set; }
        public string? BarNumber { get; set; }
    }

    public class LocationDto
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class EducationDto
    {
        public string DegreeType { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string GraduationYear { get; set; } = string.Empty;
    }

    public class CertificationDto
    {
        public string Name { get; set; } = string.Empty;
        public string IssuingOrg { get; set; } = string.Empty;
        public string YearObtained { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
    }
    public class DocumentsDto
    {
        public bool GovernmentId { get; set; }
        public string? GovernmentIdUrl { get; set; }
        public bool ProfessionalLicense { get; set; }
        public string? ProfessionalLicenseUrl { get; set; }
        public bool IdentityVerification { get; set; }
        public List<EducationCertificateDto> EducationCertificates { get; set; } = new();
    }

    public class EducationCertificateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
