using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Mapping
{
    public class BasicInfoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Lawyer, BasicInfoDataDto>()
                .Map(dest => dest.PracticeAreas, src => src.Specializations.Adapt<List<SpecializationOptionDto>>())
                .Map(dest => dest.ProfileImage, src => src.ProfileImage.SystemFileUrl);
        }
    }
}
