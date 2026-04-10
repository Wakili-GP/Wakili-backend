using Mapster;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Appointments.Mapping;

public class AppointmentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Appointment, AppointmentDto>()
            .Map(dest => dest.SessionDate, src => src.Slot.Date)
            .Map(dest => dest.StartTime, src => src.Slot.StartTime)
            .Map(dest => dest.EndTime, src => src.Slot.EndTime)
            .Map(dest => dest.SessionType, src => src.Slot.SessionType)
            .Map(dest => dest.LawyerId, src => src.Lawyer != null ? src.Lawyer.Id : null)
            .Map(dest => dest.LawyerFirstName, src => src.Lawyer != null ? src.Lawyer.FirstName : null)
            .Map(dest => dest.LawyerLastName, src => src.Lawyer != null ? src.Lawyer.LastName : null)
            .Map(dest => dest.LawyerProfileImage, src => src.Lawyer != null && src.Lawyer.ProfileImage != null ? src.Lawyer.ProfileImage.SystemFileUrl : null);

        config.NewConfig<Appointment, LawyerReceivedAppointmentDto>()
            .Map(dest => dest.SessionDate, src => src.Slot.Date)
            .Map(dest => dest.StartTime, src => src.Slot.StartTime)
            .Map(dest => dest.EndTime, src => src.Slot.EndTime)
            .Map(dest => dest.SessionType, src => src.Slot.SessionType)
            .Map(dest => dest.ClientId, src => src.ClientId)
            .Map(dest => dest.ClientFirstName, src => src.Client != null ? src.Client.FirstName : null)
            .Map(dest => dest.ClientLastName, src => src.Client != null ? src.Client.LastName : null)
            .Map(dest => dest.ClientProfileImage, src => src.Client != null && src.Client.ProfileImage != null ? src.Client.ProfileImage.SystemFileUrl : null)
            .Map(dest => dest.ClientPhone, src => src.Client != null ? src.Client.PhoneNumber : null);
    }
}

