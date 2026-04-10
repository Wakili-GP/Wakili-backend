using Mapster;
using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Create;

public class CreateAppointmentSlotCommandHandler(IAppointmentSlotRepository appointmentSlotRepository) 
    : IRequestHandler<CreateAppointmentSlotCommand, Result<AppointmentSlotDto>>
{
    public async Task<Result<AppointmentSlotDto>> Handle(CreateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var hasOverlap = await appointmentSlotRepository.HasOverlappingSlotAsync(
            request.LawyerId, request.Date, request.StartTime, request.EndTime, null, cancellationToken);
            
        if (hasOverlap)
        {
            return Result.Failure<AppointmentSlotDto>(new Error("AppointmentSlot.Overlap", "The appointment slot overlaps with an existing slot.", 409));
        }

        var appointmentSlot = new AppointmentSlot
        {
            LawyerId = request.LawyerId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SessionType = request.SessionType
        };

        await appointmentSlotRepository.AddAsync(appointmentSlot, cancellationToken);

        return Result.Success(appointmentSlot.Adapt<AppointmentSlotDto>());
    }
}