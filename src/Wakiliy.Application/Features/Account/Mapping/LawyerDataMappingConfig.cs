using Mapster;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Account.Mapping
{
    public class LawyerDataMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Lawyer, LawyerDataDto>()
                .Map(dest => dest.ProfileImage, src => src.ProfileImage != null ? src.ProfileImage.SystemFileUrl : string.Empty)
                .Map(dest => dest.Email, src => src.Email);
        }
    }
}
