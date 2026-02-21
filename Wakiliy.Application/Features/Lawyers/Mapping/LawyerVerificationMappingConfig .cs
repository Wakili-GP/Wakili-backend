using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Lawyers.Mapping
{
    public class LawyerVerificationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Lawyer, LawyerVerificationRequestResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.SubmittedAt, src => src.LastOnboardingUpdate)
                .Map(dest => dest.Status, src => src.VerificationStatus.ToString())
                .Map(dest => dest.ProfileImageUrl,
                    src => src.ProfileImage != null
                        ? "/api/files/" + src.ProfileImage.Id
                        : null);
        }
    }
}
