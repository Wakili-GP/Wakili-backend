using System;

namespace Wakiliy.Domain.Entities;

public class AcademicQualification
{
    public Guid Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public string DegreeType { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public int GraduationYear { get; set; }

    public ICollection<UploadedFile> Documents { get; set; } = new List<UploadedFile>();

    public Lawyer Lawyer { get; set; } = default!;
}
