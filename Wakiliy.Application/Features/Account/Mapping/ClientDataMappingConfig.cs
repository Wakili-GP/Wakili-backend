using Mapster;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Account.Mapping
{
    public class ClientDataMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Client, ClientDataDto>()
                .Map(dest => dest.ProfileImage, src => src.ProfileImage != null ? src.ProfileImage.SystemFileUrl : string.Empty)
                .Map(dest => dest.MemberSince, src => src.CreatedAt.ToString("MMM yyyy"));
        }
    }
}
