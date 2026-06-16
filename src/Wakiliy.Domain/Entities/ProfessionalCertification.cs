using System;

namespace Wakiliy.Domain.Entities;

public class ProfessionalCertification
{
    public Guid Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public string CertificateName { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public int YearObtained { get; set; }

    public UploadedFile? Document { get; set; }
    public Guid? DocumentId { get; set; }
    public Lawyer Lawyer { get; set; } = default!;
}
