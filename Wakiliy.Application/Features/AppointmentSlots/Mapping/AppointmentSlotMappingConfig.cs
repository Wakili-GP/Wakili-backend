using Mapster;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.AppointmentSlots.Mapping;

public class AppointmentSlotMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppointmentSlot, AppointmentSlotDto>();
    }
}