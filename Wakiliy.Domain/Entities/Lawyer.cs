using System;
using System.Collections.Generic;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class Lawyer : AppUser
{
    public DateTime JoinedDate { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }

    public string? LicenseNumber { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; } = string.Empty;
    public string? LicenseYear { get; set; } = string.Empty;

    public ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
    public List<string> SessionTypes { get; set; } = new();
    public List<AcademicQualification> AcademicQualifications { get; set; } = new();
    public List<ProfessionalCertification> ProfessionalCertifications { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public ICollection<VerificationDocuments>? VerificationDocuments { get; set; } = new List<VerificationDocuments>();
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public int CurrentOnboardingStep { get; set; } = 1;
    public List<int> CompletedOnboardingSteps { get; set; } = new();
    public DateTime LastOnboardingUpdate { get; set; } = DateTime.UtcNow;
    public double? AverageRating { get; set; }
    public bool IsActive { get; set; }
}
