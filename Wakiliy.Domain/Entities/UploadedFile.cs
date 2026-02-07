using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wakiliy.Domain.Entities
{
    public class UploadedFile
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public AppUser Owner { get; set; } = default;
        public string FileName { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string SystemFileUrl { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public FileCategory Category { get; set; }
        public FilePurpose Purpose { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public enum FileCategory
    {
        NationalIdFront,
        NationalIdBack,
        LawyerLicense,
        EducationalCertificate,
        ProfessionalCertificate,
        ProfilePicture,
    }

    public enum FilePurpose
    {
        Verification,
        Profile,
        Other
    }

}
