using System;
using System.Collections.Generic;

namespace Wakiliy.Application.Features.Lawyers.DTOs
{
    public class LawyerProfileStatsDto 
    {
        public int NumOfAppointmentsCompleted { get; set; }
        public int YearsOfExperience { get; set; }
        public int? ArticlesPublishedCount { get; set; }
        public double ClientRatingAverage { get; set; }
        public int ReviewsTotal { get; set; }
    }

    public class LawyerProfileCoreDto
    {
        public string Id { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<string> PracticeAreas { get; set; } = new();
        public LawyerProfileStatsDto Stats { get; set; } = new();
    }

    public class LawyerSessionPricingDto
    {
        public decimal PhonePrice { get; set; }
        public decimal OfficePrice { get; set; }
        // 0: InOffice, 1: Phone
        public List<int> AvailableSessionTypes { get; set; } = new();
    }

    public class LawyerWorkExperienceDto
    {
        public string JobTitle { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string StartYear { get; set; } = string.Empty;
        public string EndYear { get; set; } = string.Empty;
        public bool IsCurrentJob { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class LawyerEducationDto
    {
        public string DegreeType { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public string? GraduationYear { get; set; }
    }

    public class LawyerCertificationDto
    {
        public string CertificateName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string YearObtained { get; set; } = string.Empty;
    }

    public class PublicLawyerProfileResponseDto
    {
        public LawyerProfileCoreDto Profile { get; set; } = new();
        public LawyerSessionPricingDto Pricing { get; set; } = new();
        public List<LawyerWorkExperienceDto> WorkHistory { get; set; } = new();
        public List<LawyerEducationDto> Education { get; set; } = new();
        public List<LawyerCertificationDto> Certifications { get; set; } = new();
    }
}