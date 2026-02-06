using System;
using System.Collections.Generic;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class Lawyer : AppUser
{
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime JoinedDate { get; set; }

    // ===== Step 1: Basic Info =====
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
    public List<string> SessionTypes { get; set; } = new();

    // ===== Step 2: Education =====
    public List<AcademicQualification> AcademicQualifications { get; set; } = new();
    public List<ProfessionalCertification> ProfessionalCertifications { get; set; } = new();

    // ===== Step 3: Experience =====
    public List<WorkExperience> WorkExperiences { get; set; } = new();

    // ===== Step 4: Verification =====
    public VerificationDocuments? VerificationDocuments { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

    // ===== Onboarding Progress =====
    public int CurrentOnboardingStep { get; set; } = 1;
    public List<int> CompletedOnboardingSteps { get; set; } = new();
    public DateTime LastOnboardingUpdate { get; set; } = DateTime.UtcNow;

    public double? AverageRating { get; set; }
    public bool IsActive { get; set; }
}
