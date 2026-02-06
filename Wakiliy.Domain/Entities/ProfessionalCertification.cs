using System;

namespace Wakiliy.Domain.Entities;

public class ProfessionalCertification
{
    public Guid Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public string CertificateName { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public int YearObtained { get; set; }
    public string DocumentPath { get; set; } = string.Empty;

    public Lawyer Lawyer { get; set; } = default!;
}
