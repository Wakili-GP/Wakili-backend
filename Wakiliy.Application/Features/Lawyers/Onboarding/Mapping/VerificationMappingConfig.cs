using Mapster;
using System.Linq;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Mapping
{
    public class VerificationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Lawyer, VerificationDocumentsDto>()
                .Map(dest => dest.NationalIdFront,
                     src => src.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdFront).File.SystemFileUrl)
                .Map(dest => dest.NationalIdBack,
                     src => src.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdBack).File.SystemFileUrl)
                .Map(dest => dest.LawyerLicense,
                     src => new LawyerLicenseDto
                     {
                         LicensePath = src.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.LawyerLicense).File.SystemFileUrl,
                         IssuingAuthority = src.IssuingAuthority,
                         LicenseNumber = src.LicenseNumber,
                         LicenseYear = src.LicenseYear
                     })
                .Map(dest => dest.EducationalCertificates,
                     src => src.VerificationDocuments.Where(d => d.Type == VerificationDocumentType.EducationalCertificate).Select(d => d.File.SystemFileUrl).ToList())
                .Map(dest => dest.ProfessionalCertificates,
                     src => src.VerificationDocuments.Where(d => d.Type == VerificationDocumentType.ProfessionalCertificate).Select(d => d.File.SystemFileUrl).ToList());
        }
    }
}
