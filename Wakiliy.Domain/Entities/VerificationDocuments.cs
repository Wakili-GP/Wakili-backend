using System;
using System.Collections.Generic;

namespace Wakiliy.Domain.Entities;

public class VerificationDocuments
{
    public Guid Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;

    public UploadedDocument NationalIdFront { get; set; } = new();
    public UploadedDocument NationalIdBack { get; set; } = new();
    public UploadedDocument LawyerLicense { get; set; } = new();

    public List<UploadedDocument> EducationalCertificates { get; set; } = new();
    public List<UploadedDocument> ProfessionalCertificates { get; set; } = new();

    public Lawyer Lawyer { get; set; } = default!;
}
