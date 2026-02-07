using System;
using System.Collections.Generic;

namespace Wakiliy.Domain.Entities;

public class VerificationDocuments
{
    public Guid Id { get; set; }

    public string LawyerId { get; set; } = default!;
    public Lawyer Lawyer { get; set; } = default!;

    public Guid? FileId { get; set; }
    public UploadedFile? File { get; set; }

    public VerificationDocumentType Type { get; set; }
}

public enum VerificationDocumentType
{
    NationalIdFront,
    NationalIdBack,
    LawyerLicense,
    EducationalCertificate,
    ProfessionalCertificate
}