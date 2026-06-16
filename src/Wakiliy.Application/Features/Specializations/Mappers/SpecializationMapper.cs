using Mapster; 
using Wakiliy.Application.Features.Specializations.DTOs; 
using Wakiliy.Domain.Entities; 

namespace Wakiliy.Application.Features.Specializations.Mappers;

public class SpecializationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Specialization, SpecializationResponse>()
            .Map(dest => dest.NumOfLawyers, src => src.Lawyers != null ? src.Lawyers.Count : 0);
    }
}
