using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Mapping
{
    public class ProfessionalCertificationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProfessionalCertification, ProfessionalCertificationResponseDto>()
                .Map(dest => dest.DocumentPath, src => src.Document != null ? src.Document.SystemFileUrl : null);
        }
    }
}
