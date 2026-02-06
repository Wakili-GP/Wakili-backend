using System;
using System.Collections.Generic;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Lawyers.DTOs
{
    public class LawyerDetailsResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public bool IsActive { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string? Bio { get; set; }
        public List<SpecializationOptionDto> Specializations { get; set; } = new();
        public List<string> SessionTypes { get; set; } = new();
        public List<AcademicQualification> AcademicQualifications { get; set; } = new();
        public List<ProfessionalCertification> ProfessionalCertifications { get; set; } = new();
        public List<WorkExperience> WorkExperiences { get; set; } = new();
        public VerificationDocuments? VerificationDocuments { get; set; }
        public double? AverageRating { get; set; }
        public int CurrentOnboardingStep { get; set; }
        public List<int> CompletedOnboardingSteps { get; set; } = new();
        public DateTime LastOnboardingUpdate { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}
