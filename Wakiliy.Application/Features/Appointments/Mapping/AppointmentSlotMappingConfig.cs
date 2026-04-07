using Mapster;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Appointments.Mapping;

public class AppointmentSlotMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppointmentSlot, AppointmentSlotDto>();
    }
}