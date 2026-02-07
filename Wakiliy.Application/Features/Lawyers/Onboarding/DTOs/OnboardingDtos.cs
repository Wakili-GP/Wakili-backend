using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Wakiliy.Application.Features.Specializations.DTOs;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

public class OnboardingStepResponse<TData>
{
    public string Message { get; set; } = string.Empty;
    public OnboardingProgressDto<TData> Progress { get; set; } = default!;
}

public class OnboardingProgressDto<TData>
{
    public int CurrentStep { get; set; }
    public IReadOnlyCollection<int> CompletedSteps { get; set; } = Array.Empty<int>();
    public TData Data { get; set; } = default!;
    public DateTime LastUpdated { get; set; }
}

public class BasicInfoDataDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
   
    public List<SpecializationOptionDto> PracticeAreas { get; set; } = new();
    public List<string> SessionTypes { get; set; } = new();
}

public class EducationDataDto
{
    public List<AcademicQualificationDto> AcademicQualifications { get; set; } = new();
    public List<ProfessionalCertificationResponseDto> ProfessionalCertifications { get; set; } = new();
}

public class AcademicQualificationDto
{
    public string DegreeType { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public string GraduationYear { get; set; } = string.Empty;
}

public class ProfessionalCertificationDto
{
    public string CertificateName { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public string YearObtained { get; set; } = string.Empty;
    public IFormFile Document { get; set; } = default!;
}

public class ProfessionalCertificationResponseDto
{
    public string CertificateName { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public string YearObtained { get; set; } = string.Empty;
    public string DocumentPath { get; set; } = default!;
}

public class ExperienceDataDto
{
    public List<WorkExperienceDto> WorkExperiences { get; set; } = new();
}

public class WorkExperienceDto
{
    public string JobTitle { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string StartYear { get; set; } = string.Empty;
    public string? EndYear { get; set; }
    public bool IsCurrentJob { get; set; }
    public string? Description { get; set; }
}

public class VerificationDocumentsDto
{
    public string? NationalIdFront { get; set; }
    public string? NationalIdBack { get; set; }
    public string? LawyerLicense { get; set; }
    public List<string> EducationalCertificates { get; set; } = new();
    public List<string> ProfessionalCertificates { get; set; } = new();
}

public class UploadedDocumentDto
{
    public string? File { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
