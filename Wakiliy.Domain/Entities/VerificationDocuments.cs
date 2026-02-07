using System;
using System.Collections.Generic;

namespace Wakiliy.Domain.Entities;

public class VerificationDocuments
{
    public Guid Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;

    public string NationalIdFrontUrl { get; set; } = string.Empty;
    public string NationalIdBackUrl { get; set; } = string.Empty;
    public string LawyerLicenseUrl { get; set; } = string.Empty;

    public List<string> EducationalCertificatesUrls { get; set; } = new();
    public List<string> ProfessionalCertificatesUrls { get; set; } = new();

    public Lawyer Lawyer { get; set; } = default!;
}
