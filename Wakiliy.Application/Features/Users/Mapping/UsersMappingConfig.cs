using Mapster;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Common.Models;

namespace Wakiliy.Application.Features.Users.Mapping
{
    public class UsersMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserReadModel, UserListItemDto>()
            .Map(dest => dest.Status, src => src.Status.ToString());
        }
    }
}
